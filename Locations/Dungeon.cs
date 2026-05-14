using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Locations;

public abstract class Dungeon : Location
{
    public IPanel RetreatPanel { get; set; }

    protected Dungeon(World world, string name, string description, IPanel retreatPanel) : base(world, name, description)
    {
        RetreatPanel = retreatPanel;
    }

    public abstract void Enter(GameEngine engine);

    public override void Update(float deltaTime, GameEngine engine)
    {
        var data = engine.State.DungeonData;
        if (data.IsEventActive && data.Rooms.Count > data.CurrentRoomIndex)
        {
            var room = data.Rooms[data.CurrentRoomIndex];
            if (room.EventInstance != null)
            {
                room.EventInstance.Update(deltaTime, engine);
            }
        }
    }

    public override void Render(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        var p = engine.State.Player;
        
        int width = 80;
        try { width = Console.WindowWidth - 1; } catch { }

        Console.WriteLine($"=== {Name} ===".PadRight(width));
        string playerStats = $"{p.Name} | Живот: {p.Hp}/{p.MaxHp} | Айрян: {p.Mp}/{p.MaxMp}";
        if (p.Hp <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
        }
        Console.WriteLine();
        
        if (data.Rooms.Count > 0)
        {
            string progress = new string('-', data.CurrentRoomIndex) + "o" + new string('-', Math.Max(0, data.Rooms.Count - data.CurrentRoomIndex - 1));
            Console.WriteLine($"Напредък: [{progress}] (Стая {data.CurrentRoomIndex + 1}/{data.Rooms.Count})");
            
            if (data.CurrentRoomIndex < data.Rooms.Count)
            {
                var currentRoom = data.Rooms[data.CurrentRoomIndex];
                
                if (data.IsEventActive && currentRoom.EventInstance != null)
                {
                    Console.WriteLine();
                    currentRoom.EventInstance.Render(engine);
                    return; // Stop rendering dungeon options
                }

                if (currentRoom.IsCleared)
                {
                    Console.WriteLine("\nСтаята е изчистена.");
                }
                else if (currentRoom.EventType == 0 && currentRoom.EventInstance != null)
                {
                    Console.WriteLine("\nПредстои събитие!");
                }
                else if (currentRoom.EventType == 0)
                {
                    Console.WriteLine("\nСтаята изглежда празна.");
                }
                else if (currentRoom.EventType == 3)
                {
                    Console.WriteLine("\nПредстои битка с БОС!");
                }
                else
                {
                    Console.WriteLine("\nПредстои битка!");
                }
            }
        }

        Console.WriteLine("\nВъзможни действия:");
        foreach (var option in Options)
        {
            Console.WriteLine($" {option.Id}. {option.Text}");
        }
    }

    public override void ProcessInput(string input, GameEngine engine)
    {
        var data = engine.State.DungeonData;

        if (data.IsEventActive && data.Rooms.Count > data.CurrentRoomIndex)
        {
            var room = data.Rooms[data.CurrentRoomIndex];
            if (room.EventInstance != null)
            {
                room.EventInstance.ProcessInput(input, engine);
                if (!data.IsEventActive) room.IsCleared = true;
                return; // Event takes inputs
            }
        }

        if (!InputHandler.Handle(input, Options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }

    protected void GoForward(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        if (data.Rooms.Count == 0) return;

        var currentRoom = data.Rooms[data.CurrentRoomIndex];
        if (!currentRoom.IsCleared)
        {
            if (currentRoom.EventType > 0)
            {
                currentRoom.IsCleared = true; 
                BattleManager.StartBattle(engine, currentRoom.EnemiesToSpawn, this, currentRoom.LootMultiplier);
            }
            else if (currentRoom.EventInstance != null)
            {
                currentRoom.IsCleared = true;
                data.IsEventActive = true;
                currentRoom.EventInstance.OnOpen(engine);
            }
            else
            {
                currentRoom.IsCleared = true;
                AdvanceRoom(engine);
            }
        }
        else
        {
            AdvanceRoom(engine);
        }
    }

    protected void AdvanceRoom(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex++;
        if (data.CurrentRoomIndex >= data.Rooms.Count)
        {
            // Fully cleared!
            PerformEscape(engine);
        }
    }

    protected void PerformEscape(GameEngine engine)
    {
        engine.State.Player.Status.ClearAll();
        engine.State.Player.RecalcStats();
        Escape(engine);
    }

    protected abstract void Escape(GameEngine engine);
}
