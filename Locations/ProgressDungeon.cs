using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Events;

namespace Harduni.Locations;

public class ProgressDungeon : Dungeon
{
    private static readonly Random _rand = new();

    public ProgressDungeon(World world) : base(world, "Подземие на Прогрес", "Изключително дълго и трудно подземие.")
    {
    }

    public override void InitLinks()
    {
        Options.Add(new Option(1, "Напред", "Продължава към следващата стая или битка.", (engine) => GoForward(engine)));
        Options.Add(new Option(2, "Бягство", "Връща ви в предходната локация.", (engine) => Escape(engine)));
    }

    public override void Enter(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>();

        for (int i = 0; i < 21; i++)
        {
            if (i == 10)
            {
                // Shop event
                if (engine.State.Flags.ContainsKey("shop_unlocked"))
                {
                    rooms.Add(new Room(0, null, new BreakEvent()));
                }
                else
                {
                    rooms.Add(new Room(0, null, new ShopUnlockEvent()));
                }
                continue;
            }

            if (i == 20)
            {
                // Boss room placeholder
                rooms.Add(new Room(3, new List<Enemy> { new OligofrenBoss(), new StuckProgrammer() }));
                continue;
            }

            var enemies = new List<Enemy>();
            
            if (i < 4) // 0-3
            {
                enemies.Add(_rand.Next(2) == 0 ? new Programmer() : new SmellyProgrammer());
            }
            else if (i < 7) // 4-6
            {
                enemies.Add(new Programmer());
                enemies.Add(new SmellyProgrammer());
            }
            else if (i < 10) // 7-9
            {
                enemies.Add(new StuckProgrammer());
            }
            else if (i < 14) // 11-13
            {
                enemies.Add(new StuckProgrammer());
                enemies.Add(new Programmer());
            }
            else if (i < 17) // 14-16
            {
                enemies.Add(new StuckProgrammer());
                enemies.Add(new StuckProgrammer());
            }
            else // 17-19
            {
                enemies.Add(new StuckProgrammer());
                enemies.Add(new StuckProgrammer());
                enemies.Add(_rand.Next(2) == 0 ? new Programmer() : new SmellyProgrammer());
            }

            var room = new Room(1, enemies);
            // Higher loot in later rooms
            if (i > 10) room.LootMultiplier = 1.5f;
            if (i > 15) room.LootMultiplier = 2.0f;
            
            rooms.Add(room);
        }

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.ProgressRoom);
    }
}
