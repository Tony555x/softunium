using System;
using Harduni.Core;

namespace Harduni.Events;

public class OldLibraryEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== Стара Библиотека ===");
        VConsole.WriteLine("Намирате прашна секция с книги за Fortran. Усещате тежестта на миналото.");
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
