using System;
using Harduni.Core;

namespace Harduni.Events;

public class DarkEchoEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Ехо в мрака ===");
        Console.WriteLine("Чувате далечен звук от механична клавиатура. Някой още работи...");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
