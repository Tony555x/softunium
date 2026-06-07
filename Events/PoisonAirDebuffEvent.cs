using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Events;

public class PoisonAirDebuffEvent : IPanel
{
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        _options.Clear();
        _options.Add(new Option(1, "Продължи", "Навлезте в отровната мъгла.", (eng) => 
        {
            eng.State.Player.Status.ApplyStatus(new DecayStatus(10));
            eng.State.Player.RecalcStats();
            eng.State.DungeonData.IsEventActive = false;
        }));
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== ОТРОВНА МЪГЛА ===");
        Console.WriteLine("От другата страна на моста всичко е по-тъмно и въздухът е отровен. Не знаете колко време можете да оцелеете. (Получавате 10 разграждане).");
        
        Console.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
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
