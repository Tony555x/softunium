using System;
using Harduni.Core;

namespace Harduni.Events;

public class RelicUnlockEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        engine.State.Flags["relics_unlocked"] = "true";
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== Древно Съкровище ===");
        Console.WriteLine("Намирате странен сандък, съдържащ древни реликви.");
        Console.WriteLine("Отключихте системата за Реликви!");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
