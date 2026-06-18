using System;
using Harduni.Core;

namespace Harduni.Events;

public class DarkEchoEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== Ехо в мрака ===");
        VConsole.WriteLine("Чувате далечен звук от механична клавиатура. Някой още работи...");
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
