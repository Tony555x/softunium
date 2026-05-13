using System;
using Harduni.Core;

namespace Harduni.Events;

public class OldLibraryEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Стара Библиотека ===");
        Console.WriteLine("Намирате прашна секция с книги за Fortran. Усещате тежестта на миналото.");
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
