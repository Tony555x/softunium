using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Events;

namespace Harduni.Locations;

public class TeamworkDungeon : Dungeon
{
    public TeamworkDungeon(World world) : base(world, "Подземие на Тиймуърк", "Място за изграждане на екипен дух и груба сила.", world.TeamworkRoom)
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
            new Room(0, null, new TeamworkEntranceEvent()),

            // Room 1: Combat 1
            new Room(1, new List<Enemy> { new Delta() }),

            // Room 2: Combat 2
            new Room(1, new List<Enemy> { new Mangal() }),

            // Room 3: Protein Event (replacing Mushroom)
            new Room(0, null, new ProteinEvent()),

            // Room 4: Combat 3
            new Room(1, new List<Enemy> { new Mangal() }),

            // Room 5: Combat 4
            new Room(1, new List<Enemy> { new Beta(), new Beta(), new Beta(), new Beta(), new Beta(), new Beta() }),

            // Room 6: Combat 5
            new Room(1, new List<Enemy> { new Delta(), new Beta(), new Beta() }),

            // Room 7: Andrew Tate Shrine (replacing AI Temple)
            new Room(0, null, new AndrewTateShrineEvent()),

            // Room 8: Combat 6
            new Room(1, new List<Enemy> { new Delta(), new Mangal() }) { LootMultiplier = 1.5f },

            // Room 9: Combat 7
            new Room(1, new List<Enemy> { new Delta(), new Delta() }) { LootMultiplier = 1.5f },

            // Room 10: Combat 8
            new Room(1, new List<Enemy> { new Mangal(), new Mangal() }) { LootMultiplier = 1.5f },

            // Room 11: Casino Event (after combat 8)
            new Room(0, null, new CasinoEvent()),

            // Room 12: Combat 9
            new Room(1, new List<Enemy> { new Delta(), new Delta(), new Delta() }) { LootMultiplier = 2f },

            // Room 13: Combat 10
            new Room(1, new List<Enemy> { new Mangal(), new Mangal(), new Mangal() }) { LootMultiplier = 2f },

            // Room 14: Combat 11
            new Room(1, new List<Enemy> { new Delta(), new Mangal(), new Mangal() }) { LootMultiplier = 2f },

            // Room 15: Shop Unlock
            new Room(0, null, new StoyanovEvent()),

            // Room 16: Combat 12 (First Hamuk)
            new Room(1, new List<Enemy> { new Hamuk() }),

            // Room 17: Aura of Hunger (replacing AfterShopRest)
            new Room(0, null, new AuraOfHungerEvent()),

            // Room 18: Combat 13
            new Room(1, new List<Enemy> { new Hamuk() }),

            // Room 19: Combat 14
            new Room(1, new List<Enemy> { new Hamuk(), new Delta() }) { LootMultiplier = 1.5f },

            // Room 20: Teamwork Placeholder (replacing BrokenTerminal)
            new Room(0, null, new TeamworkPlaceholderEvent()),

            // Room 21: Combat 15
            new Room(1, new List<Enemy> { new Hamuk(), new Mangal() }) { LootMultiplier = 1.5f },

            // Room 22: Combat 16
            new Room(1, new List<Enemy> { new Hamuk(), new Delta(), new Mangal() }) { LootMultiplier = 2f },

            // Room 23: Combat 17
            new Room(1, new List<Enemy> { new Hamuk(), new Delta(), new Mangal() }) { LootMultiplier = 2f },

            // Room 24: Teamwork Placeholder (replacing DarkEcho)
            new Room(0, null, new TeamworkPlaceholderEvent()),

            // Room 25: Combat 18
            new Room(1, new List<Enemy> { new Hamuk(), new Hamuk() }) { LootMultiplier = 2f },

            // Room 26: Combat 19
            new Room(1, new List<Enemy> { new Hamuk(), new Hamuk(), new Delta(), new Mangal() }) { LootMultiplier = 2.5f },

            // Room 27: Boss Combat
            new Room(3, new List<Enemy> { new OligofrenBoss(), new Hamuk() })
        };

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.TeamworkRoom);
    }
}
