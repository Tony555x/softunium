using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;
using Harduni.Statuses;


namespace Harduni.Events;

public class ProteinEvent : IPanel
{
    private string _message = "Намирате торба с протеин на земята. Това ли е какво ползват пичагите тук за да станат нацепени?";
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

        _options.Add(new Option(1, "Бухай (+5% атака, -4 Айрян)", "Консумирайте протеина за сила.", (eng) => Consume(eng)));
        _options.Add(new Option(2, "Тръгни си", "Продължете напред.", (eng) => Leave(eng)));
    }

    private void Consume(GameEngine engine)
    {
        var player = engine.State.Player;
        if (player.Mp >= 4)
        {
            player.Mp -= 4;
            player.Status.ApplyStatus(new PersistentAtkStatus(999, 0.05f));
            player.RecalcStats();
            _message = "Бухате здраво! Усещате как мускулите ви се надуват. (+5% Атака)";
        }
        else
        {
            _message = "Нямате достатъчно Айрян (MP), за да преглътнете този сух протеин!";
        }
        
        EnsureOptions(engine);
    }

    private void Leave(GameEngine engine)
    {
        _done = true;
        engine.State.DungeonData.IsEventActive = false;
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== Торба с Протеин ===");
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
