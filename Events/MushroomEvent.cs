using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;
using Harduni.Statuses;

namespace Harduni.Events;

public class MushroomEvent : IPanel
{
    private string _message = "Стаята смърди ужасно. Докато минавате, виждате светещи гъби да растат до езеро от пот.";
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

        var flags = engine.State.Flags;
        string greenEffect = flags.ContainsKey("mushroom_green_seen") ? "Дава 'Отровен' (4 отрова при атака) за 3 битки" : "???";
        string redEffect = flags.ContainsKey("mushroom_red_seen") ? "Възстановява 30 живот" : "???";
        string bothEffect = flags.ContainsKey("mushroom_both_seen") ? "Животът става 1, но дава 'Регенерация' (2 на ход) за 3 битки" : "???";

        _options.Add(new Option(1, $"Зелена гъба ({greenEffect})", "Изяжте зелената гъба.", (eng) => EatGreen(eng)));
        _options.Add(new Option(2, $"Червена гъба ({redEffect})", "Изяжте червената гъба.", (eng) => EatRed(eng)));
        _options.Add(new Option(3, $"И двете ({bothEffect})", "Изяжте и двете гъби.", (eng) => EatBoth(eng)));
        _options.Add(new Option(4, "Подмини", "Решете да не рискувате.", (eng) => Skip(eng)));
    }

    private void EatGreen(GameEngine engine)
    {
        engine.State.Flags["mushroom_green_seen"] = "true";
        engine.State.Player.Status.ApplyStatus(new PersistentPoisonousStatus(3, 4));
        _message = "Изяждате зелената гъба. Усещате как пръстите ви започват да изпускат отровни изпарения.";
        _done = true;
        EnsureOptions(engine);
    }

    private void EatRed(GameEngine engine)
    {
        engine.State.Flags["mushroom_red_seen"] = "true";
        engine.State.Player.Heal(30);
        _message = "Изяждате червената гъба. Усещате прилив на свежест.";
        _done = true;
        EnsureOptions(engine);
    }

    private void EatBoth(GameEngine engine)
    {
        engine.State.Flags["mushroom_both_seen"] = "true";
        engine.State.Player.Hp = 1;
        engine.State.Player.Status.ApplyStatus(new PersistentRegenStatus(3, 2));
        _message = "Изяждате и двете гъби. Тялото ви се свива от болка, но усещате странна регенеративна сила.";
        _done = true;
        EnsureOptions(engine);
    }

    private void Skip(GameEngine engine)
    {
        _message = "Решавате да не рискувате и продължавате напред.";
        _done = true;
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Странни Гъби ===");
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
