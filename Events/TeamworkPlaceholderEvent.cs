using System;
using Harduni.Core;

namespace Harduni.Events;

public class TeamworkPlaceholderEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== ПРАЗНА ЗАЛА ===");
        Console.WriteLine("Тази част от подземието изглежда странно тиха...");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
