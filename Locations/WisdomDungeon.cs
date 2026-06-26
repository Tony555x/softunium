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
        engine.State.DungeonData.IsInDungeon = true;
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>
        {
            // Room 0: Combat 1
            new Room(1, new List<Enemy> { new WeakProgrammer() }),

            // Room 1: Combat 2
            new Room(1, new List<Enemy> { new Beta() }),

            // Room 2: Sign 1
            new Room(0, null, new BreakEvent(1)),

            // Room 3: Combat 3
            new Room(1, new List<Enemy> { new Beta() }),

            // Room 4: Combat 4
            new Room(2, new List<Enemy> { new WeakProgrammer(), new Beta() }) { LootMultiplier = 1.5f },

            // Room 5: Sign 2
            new Room(0, null, new BreakEvent(2)),

            // Room 6: Combat 5
            new Room(2, new List<Enemy> { new WeakProgrammer(), new Beta() }) { LootMultiplier = 1.5f },

            // Room 7: Combat 6
            new Room(2, new List<Enemy> { new WeakProgrammer(), new Beta() }) { LootMultiplier = 1.5f },

            // Room 8: Sign 3
            new Room(0, null, new BreakEvent(3)),

            // Room 9: Combat 7 (Boss)
            new Room(3, new List<Enemy> { new OligofrenBoss() }),

            // Room 10: Sign 4
            new Room(0, null, new BreakEvent(4)),

            // Room 11: Slav Event
            engine.State.Flags.ContainsKey("slav_defeated") 
                ? new Room(0, null, new BreakEvent(0)) 
                : new Room(0, null, new SlavEvent())
        };

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.WisdomRoom);
    }
}
