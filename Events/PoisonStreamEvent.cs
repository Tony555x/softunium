using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Events;

public class PoisonStreamEvent : IPanel
{
    private string _message = "Пътeката пред вас е препречена от плитък поток от отрова.";
    private bool _done = false;
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        EnsureOptions(engine);
    }

    private void EnsureOptions(GameEngine engine)
    {
        _options.Clear();
        if (_done) return;

        _options.Add(new Option(1, "Прегази (-20 Живот)", "Прегазете бавно през отровния поток.", (eng) => CrossWade(eng)));
        _options.Add(new Option(2, "Прескочи (50% за -40 Живот)", "Опитайте се да прескочите потока с един скок.", (eng) => CrossJump(eng)));
    }

    private void CrossWade(GameEngine engine)
    {
        var p = engine.State.Player;
        p.Hp = Math.Max(1, p.Hp - 20);
        _message = "Прегазихте през потока. Отровата разяжда краката ви и губите 20 Живот.";
        _done = true;
        EnsureOptions(engine);
    }

    private void CrossJump(GameEngine engine)
    {
        var p = engine.State.Player;
        var rand = new Random();
        if (rand.Next(2) == 0) // Success
        {
            _message = "Успешно прескочихте потока без да се докоснете до отровата!";
        }
        else // Failure
        {
            p.Hp = Math.Max(1, p.Hp - 40);
            _message = "Не успяхте да прескочите потока и паднахте вътре! Изгарянията ви костват 40 Живот.";
        }
        _done = true;
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== ПОТОК ОТ ОТРОВА ===");
        Console.WriteLine(_message);
        
        if (_done)
        {
            Console.WriteLine("\n[Натиснете Enter за продължаване]");
            return;
        }

        Console.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (_done)
        {
            engine.State.DungeonData.IsEventActive = false;
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
