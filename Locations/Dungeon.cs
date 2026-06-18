using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;
using Harduni.Statuses;


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
        try { width = VConsole.WindowWidth - 1; } catch { }

        VConsole.WriteLine($"=== {Name} ===".PadRight(width));
        string pStatusStr = p.Status.GetCombinedDisplayString();
        string playerStats = $"{p.BattleName} | Живот: {p.Hp}/{p.MaxHp} | Айрян: {p.Mp}/{p.MaxMp} {pStatusStr}";
        
        if (p.Hp <= 0)
        {
            VConsole.ForegroundColor = ConsoleColor.Red;
            VConsole.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
            VConsole.ResetColor();
        }
        else
        {
            VConsole.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
        }
        VConsole.WriteLine();
        
        if (data.Rooms.Count > 0)
        {
            string progress = new string('-', data.CurrentRoomIndex) + "o" + new string('-', Math.Max(0, data.Rooms.Count - data.CurrentRoomIndex - 1));
            VConsole.WriteLine($"Напредък: [{progress}] (Стая {data.CurrentRoomIndex + 1}/{data.Rooms.Count})");
            
            if (data.CurrentRoomIndex < data.Rooms.Count)
            {
                var currentRoom = data.Rooms[data.CurrentRoomIndex];
                
                if (data.IsEventActive && currentRoom.EventInstance != null)
                {
                    VConsole.WriteLine();
                    currentRoom.EventInstance.Render(engine);
                    return; // Stop rendering dungeon options
                }

                if (currentRoom.IsCleared)
                {
                    VConsole.WriteLine("\nСтаята е изчистена.");
                }
                else if (currentRoom.EventType == 0 && currentRoom.EventInstance != null)
                {
                    VConsole.WriteLine("\nПредстои събитие!");
                }
                else if (currentRoom.EventType == 0)
                {
                    VConsole.WriteLine("\nСтаята изглежда празна.");
                }
                else if (currentRoom.EventType == 3)
                {
                    VConsole.WriteLine("\nПредстои битка с БОС!");
                }
                else
                {
                    VConsole.WriteLine("\nПредстои битка!");
                }
            }
        }

        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var option in Options)
        {
            VConsole.WriteLine($" {option.Id}. {option.Text}");
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
