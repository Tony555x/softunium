using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Events;

public class ObservedCameraEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        var p = engine.State.Player;
        p.Status.ApplyStatus(new PersistentObservedStatus());
        p.RecalcStats();
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== КАМЕРА ===");
        VConsole.WriteLine("Имаш чувството че биваш наблюдаван.");
        VConsole.WriteLine("Разбираш откъде: камерата в ъгъла на стаята те гледа.");
        VConsole.WriteLine("\n(Уменията ви струват +1 Айрян.)");
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
