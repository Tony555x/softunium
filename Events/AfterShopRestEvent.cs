using System;
using Harduni.Core;

namespace Harduni.Events;

public class AfterShopRestEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Почивка след пазар ===");
        Console.WriteLine("Сядате на един изгнил стол, за да проверите новите си придобивки.");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
