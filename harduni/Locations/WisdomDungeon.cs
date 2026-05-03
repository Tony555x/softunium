using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Panels;

namespace Harduni.Locations;

public class WisdomDungeon : Dungeon
{
    public WisdomDungeon(World world) : base(world, "Подземие на Уйздом", "")
    {
    }

    public override void InitLinks()
    {
        Options.Add(new Option(1, "Напред", "Продължава към следващата стая или битка.", (engine) => GoForward(engine)));
        Options.Add(new Option(2, "Бягство", "Връща ви в предходната локация.", (engine) => Escape(engine)));
    }

    public override void Initialize(GameEngine engine)
    {
        var rooms = new List<Room>();
        int[] layout = { 1, 1, 0, 1, 2, 0, 2, 2, 0, 3 };

        for (int i = 0; i < layout.Length; i++)
        {
            int eventType = layout[i];
            
            if (eventType > 0)
            {
                int roomIndex = i;
                Func<List<Enemy>> spawner = () => 
                {
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
                        enemies.Add(new BossSenior());
                    }
                    return enemies;
                };
                
                rooms.Add(new Room(eventType, spawner));
            }
            else
            {
                rooms.Add(new Room(0, null, new ExampleEvent()));
            }
        }

        engine.State.DungeonData.Rooms = rooms;
        Reset(engine);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.WisdomRoom);
    }
}
