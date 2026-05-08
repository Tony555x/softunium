using System;
using Harduni.Core;

namespace Harduni.Events;

public class BreakEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Междучасие ===");
        Console.WriteLine("Настъпва междучасие в стаята. Усещаш как духът ти се възстановява!");
        Console.WriteLine(" 1. Ядене (+5 Живот)");
        Console.WriteLine(" 2. Спане (+2 Айрян)");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        var data = engine.State.DungeonData;
        var p = engine.State.Player;

        if (input == "1")
        {
            p.Heal(5);
            data.IsEventActive = false;
        }
        else if (input == "2")
        {
            p.Mp = Math.Min(p.MaxMp, p.Mp + 2);
            data.IsEventActive = false;
        }
    }
}
