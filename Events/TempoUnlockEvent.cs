using System;
using Harduni.Core;

namespace Harduni.Events;

public class TempoUnlockEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        engine.State.Flags["tempo_unlocked"] = "true";
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== ЗАГАДЪЧЕН ТАКТ ===");
        Console.WriteLine("Намирате стая, пълна с метрономи, които тиктакат в перфектен синхрон.");
        Console.WriteLine("Изведнъж усещате как вашият вътрешен ритъм се променя.");
        Console.WriteLine("\n(Отключихте Темпо!)");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
