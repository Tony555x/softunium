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
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>();

        for (int i = 0; i < 27; i++)
        {
            Room room = null;

            if (i == 0 || i == 1) // Combat 1, 2
            {
                room = new Room(1, new List<Enemy> { i % 2 == 0 ? new Kifla() : new Zubarka() });
            }
            else if (i == 2) // Mushroom Event equivalent
            {
                room = new Room(0, null, new BreakEvent()); // Placeholder for now
            }
            else if (i == 3) // Combat 3
            {
                room = new Room(1, new List<Enemy> { new Zubarka() });
            }
            else if (i == 4) // Combat 4
            {
                room = new Room(1, new List<Enemy> { new Beta(), new Beta(), new Beta(), new Beta(), new Beta() });
            }
            else if (i == 5) // Combat 5
            {
                room = new Room(1, new List<Enemy> { new Kifla(), new Beta(), new Beta() });
            }
            else if (i == 6) // TikTok Shrine Event
            {
                room = new Room(0, null, new TikTokShrineEvent());
            }
            else if (i == 7) // Combat 6
            {
                room = new Room(1, new List<Enemy> { new Kifla(), new Zubarka() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 8) // Combat 7
            {
                room = new Room(1, new List<Enemy> { new Kifla(), new Kifla() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 9) // Combat 8
            {
                room = new Room(1, new List<Enemy> { new Zubarka(), new Zubarka() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 10) // Flavor Event 1
            {
                room = new Room(0, null, new BreakEvent()); // Placeholder
            }
            else if (i == 11) // Combat 9
            {
                room = new Room(1, new List<Enemy> { new Kifla(), new Kifla(), new Kifla() });
                room.LootMultiplier = 2f;
            }
            else if (i == 12) // Combat 10
            {
                room = new Room(1, new List<Enemy> { new Zubarka(), new Zubarka(), new Zubarka() });
                room.LootMultiplier = 2f;
            }
            else if (i == 13) // Combat 11
            {
                room = new Room(1, new List<Enemy> { new Kifla(), new Zubarka(), new Zubarka() });
                room.LootMultiplier = 2f;
            }
            else if (i == 14) // Relic Unlock Event
            {
                if (engine.State.Flags.ContainsKey("relics_unlocked"))
                {
                    room = new Room(0, null, new BreakEvent());
                }
                else
                {
                    room = new Room(0, null, new RelicUnlockEvent());
                }
            }
            else if (i == 15) // Combat 12
            {
                room = new Room(1, new List<Enemy> { new Dizainerka() });
            }
            else if (i == 16) // DEBUFF EVENT
            {
                room = new Room(0, null, new BreakEvent()); // Placeholder
            }
            else if (i == 17) // Combat 13
            {
                room = new Room(1, new List<Enemy> { new Dizainerka() });
            }
            else if (i == 18) // Combat 14
            {
                room = new Room(1, new List<Enemy> { new Dizainerka(), new Kifla() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 19) // Flavor Event 3
            {
                room = new Room(0, null, new BreakEvent()); // Placeholder
            }
            else if (i == 20) // Combat 15
            {
                room = new Room(1, new List<Enemy> { new Dizainerka(), new Zubarka() });
                room.LootMultiplier = 1.5f;
            }
            else if (i == 21) // Combat 16
            {
                room = new Room(1, new List<Enemy> { new Dizainerka(), new Kifla(), new Zubarka() });
                room.LootMultiplier = 2f;
            }
            else if (i == 22) // Combat 17
            {
                room = new Room(1, new List<Enemy> { new Dizainerka(), new Kifla(), new Zubarka() });
                room.LootMultiplier = 2f;
            }
            else if (i == 23) // Flavor Event 4
            {
                room = new Room(0, null, new BreakEvent()); // Placeholder
            }
            else if (i == 24) // Combat 18
            {
                room = new Room(1, new List<Enemy> { new Dizainerka(), new Dizainerka() });
                room.LootMultiplier = 2f;
            }
            else if (i == 25) // Combat 19
            {
                room = new Room(1, new List<Enemy> { new Dizainerka(), new Dizainerka(), new Kifla(), new Zubarka() });
                room.LootMultiplier = 2.5f;
            }
            else if (i == 26) // Boss Combat
            {
                room = new Room(3, new List<Enemy> { new OligofrenBoss(), new Dizainerka() });
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
        engine.ChangeRootPanel(World.LeadershipRoom);
    }
}
