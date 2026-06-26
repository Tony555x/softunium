using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Events;

public class TeamworkEntranceEvent : IPanel
{
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        _options.Clear();
        _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => eng.State.DungeonData.IsEventActive = false));
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== ВЛИЗАНЕ В ТИЙМУЪРК ===");
        VConsole.WriteLine("Влизайки в стаята се озовавате в град от панелки. Има нацепени пичове с тояги наоколо.");
        
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
