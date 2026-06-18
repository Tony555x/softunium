using System;
using Harduni.Core;

namespace Harduni.Events;

public class BreakEvent : IPanel
{
    private readonly int _id;

    public BreakEvent(int id = 0)
    {
        _id = id;
    }

    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        if (_id == 1)
        {
            VConsole.WriteLine("=== Табела: Щети ===");
            VConsole.WriteLine("На табелата пише:");
            VConsole.WriteLine("Щетите от атаки се намаляват от защитата на целта разделена на две (Защита / 2).");
        }
        else if (_id == 2)
        {
            VConsole.WriteLine("=== Табела: Инвентар ===");
            VConsole.WriteLine("На табелата пише:");
            VConsole.WriteLine("Можете да отворите своите характеристики с '@'. От там можете да използвате умения като 'Лечение', да запазвате играта и други.");
        }
        else if (_id == 3)
        {
            VConsole.WriteLine("=== Табела: Изчакване ===");
            VConsole.WriteLine("На табелата пише:");
            VConsole.WriteLine("Някои умения имат изчакване, отбелязано с (~). Трябва да изчакате този брой ходове в началото на всяка битка и след всяко използване. Извън битка няма изчаквания.");
        }
        else
        {
            VConsole.WriteLine("=== Междучасие ===");
            VConsole.WriteLine("Стаята е празна и тиха.");
        }

        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
