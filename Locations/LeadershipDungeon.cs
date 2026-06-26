using System.Collections.Generic;
using Harduni.Core;
using Harduni.Events;
using Harduni.Enemies;

namespace Harduni.Locations;

public class LeadershipDungeon : Dungeon
{
    public LeadershipDungeon(World world) : base(world, "Подземие Лийдършип", "Подземие за истински лидери.", world.LeadershipRoom)
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Напред", "Продължава към следващата стая.", (engine) => GoForward(engine)));
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
            new Room(1, new List<Enemy> { new Kifla() }),

            // Room 1: Combat 2
            new Room(1, new List<Enemy> { new Zubarka() }),

            // Room 2: Mushroom Event equivalent
            new Room(0, null, new BreakEvent()),

            // Room 3: Combat 3
            new Room(1, new List<Enemy> { new Zubarka() }),

            // Room 4: Combat 4
            new Room(1, new List<Enemy> { new Beta(), new Beta(), new Beta(), new Beta(), new Beta() }),

            // Room 5: Combat 5
            new Room(1, new List<Enemy> { new Kifla(), new Beta(), new Beta() }),

            // Room 6: TikTok Shrine Event
            new Room(0, null, new TikTokShrineEvent()),

            // Room 7: Combat 6
            new Room(1, new List<Enemy> { new Kifla(), new Zubarka() }) { LootMultiplier = 1.5f },

            // Room 8: Combat 7
            new Room(1, new List<Enemy> { new Kifla(), new Kifla() }) { LootMultiplier = 1.5f },

            // Room 9: Combat 8
            new Room(1, new List<Enemy> { new Zubarka(), new Zubarka() }) { LootMultiplier = 1.5f },

            // Room 10: Flavor Event 1
            new Room(0, null, new BreakEvent()),

            // Room 11: Combat 9
            new Room(1, new List<Enemy> { new Kifla(), new Kifla(), new Kifla() }) { LootMultiplier = 2f },

            // Room 12: Combat 10
            new Room(1, new List<Enemy> { new Zubarka(), new Zubarka(), new Zubarka() }) { LootMultiplier = 2f },

            // Room 13: Combat 11
            new Room(1, new List<Enemy> { new Kifla(), new Zubarka(), new Zubarka() }) { LootMultiplier = 2f },

            // Room 14: Relic Unlock Event
            new Room(0, null, new PeshoNpcEvent()),

            // Room 15: Combat 12
            new Room(1, new List<Enemy> { new Dizainerka() }),

            // Room 16: DEBUFF EVENT
            new Room(0, null, new BreakEvent()),

            // Room 17: Combat 13
            new Room(1, new List<Enemy> { new Dizainerka() }),

            // Room 18: Combat 14
            new Room(1, new List<Enemy> { new Dizainerka(), new Kifla() }) { LootMultiplier = 1.5f },

            // Room 19: Flavor Event 3
            new Room(0, null, new BreakEvent()),

            // Room 20: Combat 15
            new Room(1, new List<Enemy> { new Dizainerka(), new Zubarka() }) { LootMultiplier = 1.5f },

            // Room 21: Combat 16
            new Room(1, new List<Enemy> { new Dizainerka(), new Kifla(), new Zubarka() }) { LootMultiplier = 2f },

            // Room 22: Combat 17
            new Room(1, new List<Enemy> { new Dizainerka(), new Kifla(), new Zubarka() }) { LootMultiplier = 2f },

            // Room 23: Flavor Event 4
            new Room(0, null, new BreakEvent()),

            // Room 24: Combat 18
            new Room(1, new List<Enemy> { new Dizainerka(), new Dizainerka() }) { LootMultiplier = 2f },

            // Room 25: Combat 19
            new Room(1, new List<Enemy> { new Dizainerka(), new Dizainerka(), new Kifla(), new Zubarka() }) { LootMultiplier = 2.5f },

            // Room 26: Boss Combat
            new Room(3, new List<Enemy> { new OligofrenBoss(), new Dizainerka() })
        };

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.LeadershipRoom);
    }
}
