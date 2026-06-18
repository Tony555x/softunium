using System;
using Harduni.Core;

namespace Harduni.Events;

public class AfterShopRestEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== Почивка след пазар ===");
        VConsole.WriteLine("Сядате на един изгнил стол, за да проверите новите си придобивки.");
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
