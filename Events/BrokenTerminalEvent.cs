using System;
using Harduni.Core;

namespace Harduni.Events;

public class BrokenTerminalEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Счупен терминал ===");
        Console.WriteLine("Виждате терминал, който постоянно изписва 'Segmentation Fault'. Тъжно.");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
