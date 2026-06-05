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
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>();

        for (int i = 0; i < 27; i++)
        {
            Room room = null;

            if (i == 0 || i == 1) // Combat 1, 2
            {
                room = new Room(1, new List<Enemy> { i % 2 == 0 ? new Delta() : new Mangal() });
            }
            else if (i == 2) // Protein Event (replacing Mushroom)
            {
                room = new Room(0, null, new ProteinEvent());
            }
            else if (i == 3) // Combat 3
            {
                room = new Room(1, new List<Enemy> { new Mangal() });
            }
            else if (i == 4) // Combat 4
            {
                room = new Room(1, new List<Enemy> { new Beta(), new Beta(), new Beta(), new Beta(), new Beta(), new Beta() });
            }
            else if (i == 5) // Combat 5
            {
                room = new Room(1, new List<Enemy> { new Delta(), new Beta(), new Beta() });
            }
            else if (i == 6) // Andrew Tate Shrine (replacing AI Temple)
            {
                room = new Room(0, null, new AndrewTateShrineEvent());
            }
            else if (i == 7) // Combat 6
            {
                room = new Room(1, new List<Enemy> { new Delta(), new Mangal() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 8) // Combat 7
            {
                room = new Room(1, new List<Enemy> { new Delta(), new Delta() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 9) // Combat 8
            {
                room = new Room(1, new List<Enemy> { new Mangal(), new Mangal() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 10) // Teamwork Placeholder (replacing OldLibrary)
            {
                room = new Room(0, null, new TeamworkPlaceholderEvent());
            }
            else if (i == 11) // Combat 9
            {
                room = new Room(1, new List<Enemy> { new Delta(), new Delta(), new Delta() });
                room.LootMultiplier = 2f;
            }
            else if (i == 12) // Combat 10
            {
                room = new Room(1, new List<Enemy> { new Mangal(), new Mangal(), new Mangal() });
                room.LootMultiplier = 2f;
            }
            else if (i == 13) // Combat 11
            {
                room = new Room(1, new List<Enemy> { new Delta(), new Mangal(), new Mangal() });
                room.LootMultiplier = 2f;
            }
            else if (i == 14) // Shop Unlock
            {
                room = new Room(0, null, new StoyanovEvent());
            }
            else if (i == 15) // Combat 12 (First Hamuk)
            {
                room = new Room(1, new List<Enemy> { new Hamuk() });
            }
            else if (i == 16) // Aura of Hunger (replacing AfterShopRest)
            {
                room = new Room(0, null, new AuraOfHungerEvent());
            }
            else if (i == 17) // Combat 13
            {
                room = new Room(1, new List<Enemy> { new Hamuk() });
            }
            else if (i == 18) // Combat 14
            {
                room = new Room(1, new List<Enemy> { new Hamuk(), new Delta() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 19) // Teamwork Placeholder (replacing BrokenTerminal)
            {
                room = new Room(0, null, new TeamworkPlaceholderEvent());
            }
            else if (i == 20) // Combat 15
            {
                room = new Room(1, new List<Enemy> { new Hamuk(), new Mangal() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 21) // Combat 16
            {
                room = new Room(1, new List<Enemy> { new Hamuk(), new Delta(), new Mangal() });
                room.LootMultiplier = 2f;
            }
            else if (i == 22) // Combat 17
            {
                room = new Room(1, new List<Enemy> { new Hamuk(), new Delta(), new Mangal() });
                room.LootMultiplier = 2f;
            }
            else if (i == 23) // Teamwork Placeholder (replacing DarkEcho)
            {
                room = new Room(0, null, new TeamworkPlaceholderEvent());
            }
            else if (i == 24) // Combat 18
            {
                room = new Room(1, new List<Enemy> { new Hamuk(), new Hamuk() });
                room.LootMultiplier = 2f;
            }
            else if (i == 25) // Combat 19
            {
                room = new Room(1, new List<Enemy> { new Hamuk(), new Hamuk(), new Delta(), new Mangal() });
                room.LootMultiplier = 2.5f;
            }
            else if (i == 26) // Boss Combat
            {
                room = new Room(3, new List<Enemy> { new OligofrenBoss(), new Hamuk() });
            }

            if (room != null)
            {
                rooms.Add(room);
            }
        }

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.TeamworkRoom);
    }
}
