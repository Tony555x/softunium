using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Events;

namespace Harduni.Locations;

public class ProgressDungeon : Dungeon
{
    private static readonly Random _rand = new();

    public ProgressDungeon(World world) : base(world, "Подземие на Прогрес", "Изключително дълго и трудно подземие.", world.ProgressRoom)
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
            // Room 0: Entrance Event
            new Room(0, null, new ProgressEntranceEvent()),

            // Room 1: Combat 1
            new Room(1, new List<Enemy> { new Programmer() }),

            // Room 2: Combat 2
            new Room(1, new List<Enemy> { new SmellyProgrammer() }),

            // Room 3: Mushroom Event
            new Room(0, null, new MushroomEvent()),

            // Room 4: Combat 3
            new Room(1, new List<Enemy> { new SmellyProgrammer() }),

            // Room 5: Combat 4
            new Room(1, new List<Enemy> { new WeakProgrammer(), new WeakProgrammer(), new WeakProgrammer(), new WeakProgrammer(), new WeakProgrammer()}),

            // Room 6: Combat 5
            new Room(1, new List<Enemy> { new Programmer(), new Beta(), new WeakProgrammer() }),

            // Room 7: AI Temple Event
            new Room(0, null, new AIShrineEvent()),

            // Room 8: Combat 6
            new Room(1, new List<Enemy> { new Programmer(), new SmellyProgrammer() }) { LootMultiplier = 1.5f },

            // Room 9: Combat 7
            new Room(1, new List<Enemy> { new Programmer(), new Programmer() }) { LootMultiplier = 1.5f },

            // Room 10: Combat 8
            new Room(1, new List<Enemy> { new SmellyProgrammer(), new SmellyProgrammer() }) { LootMultiplier = 1.5f },

            // Room 11: Poison Stream Event (after fight 8)
            new Room(0, null, new PoisonStreamEvent()),

            // Room 12: Combat 9
            new Room(1, new List<Enemy> { new Programmer(), new Programmer(), new Programmer() }) { LootMultiplier = 2f },

            // Room 13: Combat 10
            new Room(1, new List<Enemy> { new SmellyProgrammer(), new SmellyProgrammer(), new SmellyProgrammer() }) { LootMultiplier = 2f },

            // Room 14: Combat 11
            new Room(1, new List<Enemy> { new Programmer(), new SmellyProgrammer(), new SmellyProgrammer() }) { LootMultiplier = 2f },

            // Room 15: Tempo Event
            new Room(0, null, new TempoUnlockEvent()),

            // Room 16: Bridge Warning Event (before combat 12)
            new Room(0, null, new BridgeWarningEvent()),

            // Room 17: Combat 12
            new Room(1, new List<Enemy> { new StuckProgrammer() }),

            // Room 18: Poison Air Debuff Event
            new Room(0, null, new PoisonAirDebuffEvent()),

            // Room 19: Combat 13
            new Room(1, new List<Enemy> { new StuckProgrammer() }),

            // Room 20: Combat 14
            new Room(1, new List<Enemy> { new StuckProgrammer(), new Programmer() }) { LootMultiplier = 1.5f },

            // Room 21: Combat 15
            new Room(1, new List<Enemy> { new StuckProgrammer(), new SmellyProgrammer() }) { LootMultiplier = 1.5f },

            // Room 22: Clean Room / Laptop Event
            new Room(0, null, new CleanRoomLaptopEvent()),

            // Room 23: Combat 16
            new Room(1, new List<Enemy> { new StuckProgrammer(), new Programmer(), new SmellyProgrammer() }) { LootMultiplier = 2f },

            // Room 24: Combat 17
            new Room(1, new List<Enemy> { new StuckProgrammer(), new Programmer(), new SmellyProgrammer() }) { LootMultiplier = 2f },

            // Room 25: Combat 18
            new Room(1, new List<Enemy> { new StuckProgrammer(), new StuckProgrammer() }) { LootMultiplier = 2f },

            // Room 26: Combat 19
            new Room(1, new List<Enemy> { new StuckProgrammer(), new StuckProgrammer(), new Programmer(), new SmellyProgrammer() }) { LootMultiplier = 2.5f },

            // Room 27: Boss Combat
            new Room(3, new List<Enemy> { new OligofrenBoss(), new StuckProgrammer() })
        };

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.ProgressRoom);
    }
}
