using System;
using Harduni.Core;

namespace Harduni.Events;

public class TeamworkPlaceholderEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== ПРАЗНА ЗАЛА ===");
        VConsole.WriteLine("Тази част от подземието изглежда странно тиха...");
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
