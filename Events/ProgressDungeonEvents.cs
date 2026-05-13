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

    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== Странни Гъби ===");
        Console.WriteLine(_message);
        
        if (_done)
        {
            Console.WriteLine("\n[Натиснете Enter за продължаване]");
            return;
        }

        var flags = engine.State.Flags;
        string greenEffect = flags.ContainsKey("mushroom_green_seen") ? "Дава 'Гъбена Отрова' (4 отрова при атака) за 3 битки" : "???";
        string redEffect = flags.ContainsKey("mushroom_red_seen") ? "Възстановява 30 живот" : "???";
        string bothEffect = flags.ContainsKey("mushroom_both_seen") ? "Животът става 1, но дава 'Гъбена Регенерация' (2 на ход) за 3 битки" : "???";

        Console.WriteLine($" 1. Зелена гъба ({greenEffect})");
        Console.WriteLine($" 2. Червена гъба ({redEffect})");
        Console.WriteLine($" 3. И двете ({bothEffect})");
        Console.WriteLine(" 4. Подмини");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (_done)
        {
            engine.State.DungeonData.IsEventActive = false;
            return;
        }

        var p = engine.State.Player;
        var flags = engine.State.Flags;

        if (input == "1")
        {
            flags["mushroom_green_seen"] = "true";
            p.Status.ApplyStatus(new PersistentPoisonousStatus(3, 4));
            _message = "Изяждате зелената гъба. Усещате как пръстите ви започват да изпускат отровни изпарения.";
            _done = true;
        }
        else if (input == "2")
        {
            flags["mushroom_red_seen"] = "true";
            p.Heal(30);
            _message = "Изяждате червената гъба. Усещате прилив на свежест.";
            _done = true;
        }
        else if (input == "3")
        {
            flags["mushroom_both_seen"] = "true";
            p.Hp = 1;
            p.Status.ApplyStatus(new PersistentRegenStatus(3, 2));
            _message = "Изяждате и двете гъби. Тялото ви се свива от болка, но усещате странна регенеративна сила.";
            _done = true;
        }
        else if (input == "4")
        {
            _message = "Решавате да не рискувате и продължавате напред.";
            _done = true;
        }
    }
}

public class AITempleEvent : IPanel
{
    private string _message = "Намирате странен олтар, заобиколен от горяща вода. Можете да почувствате магическа аура.";
    private bool _beepsHeard = false;
    private bool _done = false;

    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        //Console.WriteLine("=== Храм на Изкуствения Интелект ===");
        Console.WriteLine(_message);

        if (_done)
        {
            Console.WriteLine("\n[Натиснете Enter за продължаване]");
            return;
        }

        if (!_beepsHeard)
        {
            Console.WriteLine(" 1. Приближи се");
        }
        else
        {
            Console.WriteLine("\nСлед секунда бийпове, се чувствате по-умни.");
            Console.WriteLine("[Натиснете Enter за продължаване]");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (_done)
        {
            engine.State.DungeonData.IsEventActive = false;
            return;
        }

        if (!_beepsHeard)
        {
            if (input == "1")
            {
                _beepsHeard = true;
                engine.State.Flags["ai_temple_magic_bonus"] = "true";
                engine.State.Player.RecalcStats(); // Trigger the stat update
                _message = "Приближавате се и чувате странни компютърни звуци в съзнанието си за секунда. След това се чувствате по-умни.";
            }
        }
        else
        {
            _done = true;
        }
    }
}

public class ProgressFlavorEvent : IPanel
{
    private readonly string _title;
    private readonly string _text;

    public ProgressFlavorEvent(string title, string text)
    {
        _title = title;
        _text = text;
    }

    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine($"\n=== {_title} ===");
        Console.WriteLine(_text);
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
