using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Events;

public class ProgressEntranceEvent : IPanel
{
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        _options.Clear();
        _options.Add(new Option(1, "продължи", "Навлезте в блатото.", (eng) => eng.State.DungeonData.IsEventActive = false));
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== ВЛИЗАНЕ В ПРОГРЕС ===");
        VConsole.WriteLine("Вие влизате в Прогрес. Веднага усещата ужасна миризма. Пред вас се разпростира голямо блато, земята е преплетена с кабели и e осеяна от локви и езера от отровни вещества.");
        
        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
