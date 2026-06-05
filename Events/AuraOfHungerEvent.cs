using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Events;

public class AuraOfHungerEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        var p = engine.State.Player;
        p.Status.ApplyStatus(new PersistentAtkStatus(999, 0.25f));
        p.Status.ApplyStatus(new PersistentDefStatus(999, -1.0f));
        p.RecalcStats();
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== АУРА НА ГЛАДА ===");
        Console.WriteLine("Усещате неистовия глад на обитателите на тази стая...");
        Console.WriteLine("Той ви заразява! Чувствате жажда за кръв, но сте много по-уязвими.");
        Console.WriteLine("\n(Получихте +25% Атака и -100% Защита персистентно!)");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
