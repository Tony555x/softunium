using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Panels;
using Harduni.Events;

namespace Harduni.Locations;

public class WisdomDungeon : Dungeon
{
    public WisdomDungeon(World world) : base(world, "Подземие на Уйздом", "", world.WisdomRoom)
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Напред", "Продължава към следващата стая или битка.", (engine) => GoForward(engine)));
        Options.Add(new Option(2, "Бягство", "Връща ви в предходната локация.", (engine) => PerformEscape(engine)));
    }

    public override void Enter(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>();
        int[] layout = { 1, 1, 0, 1, 2, 0, 2, 2, 0, 3, 0 }; // Added 0 at the end for Slav

        for (int i = 0; i < layout.Length; i++)
        {
            int eventType = layout[i];
            
            if (i == layout.Length - 1)
            {
                // Last room is Slav
                // Using flags to track if slav is defeated
                if (engine.State.Flags.ContainsKey("slav_defeated"))
                {
                    rooms.Add(new Room(0, null, new BreakEvent())); // Placeholder for empty room
                }
                else
                {
                    rooms.Add(new Room(0, null, new SlavEvent()));
                }
                continue;
            }

            if (eventType > 0)
            {
                int roomIndex = i;
                var enemies = new List<Enemy>();
                if (eventType == 1)
                {
                    enemies.Add(roomIndex % 2 == 0 ? new WeakProgrammer() : new Beta());
                }
                else if (eventType == 2)
                {
                    enemies.Add(new WeakProgrammer());
                    enemies.Add(new Beta());
                }
                else if (eventType == 3)
                {
                    enemies.Add(new OligofrenBoss());
                }
                
                var room = new Room(eventType, enemies);
                if (eventType == 2) room.LootMultiplier = 1.5f;
                rooms.Add(room);
            }
            else
            {
                rooms.Add(new Room(0, null, new BreakEvent()));
            }
        }

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.WisdomRoom);
    }
}
