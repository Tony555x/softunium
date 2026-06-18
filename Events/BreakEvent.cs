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
            Console.WriteLine("=== Табела: Щети ===");
            Console.WriteLine("На табелата пише:");
            Console.WriteLine("Щетите от атаки се намаляват от защитата на целта разделена на две (Защита / 2).");
        }
        else if (_id == 2)
        {
            Console.WriteLine("=== Табела: Инвентар ===");
            Console.WriteLine("На табелата пише:");
            Console.WriteLine("Можете да отворите своите характеристики с '@'. От там можете да използвате умения като 'Лечение', да запазвате играта и други.");
        }
        else if (_id == 3)
        {
            Console.WriteLine("=== Табела: Изчакване ===");
            Console.WriteLine("На табелата пише:");
            Console.WriteLine("Някои умения имат изчакване, отбелязано с (~). Трябва да изчакате този брой ходове в началото на всяка битка и след всяко използване. Извън битка няма изчаквания.");
        }
        else
        {
            Console.WriteLine("=== Междучасие ===");
            Console.WriteLine("Стаята е празна и тиха.");
        }

        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
