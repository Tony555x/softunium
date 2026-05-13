using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Events;

public class AITempleEvent : IPanel
{
    private string _message = "Намирате странен олтар, заобиколен от горяща вода. Можете да почувствате магическа аура.";
    private bool _beepsHeard = false;
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
        if (_done || _beepsHeard) return;

        _options.Add(new Option(1, "Приближи се", "Приближете се към олтара.", (eng) => Approach(eng)));
    }

    private void Approach(GameEngine engine)
    {
        _beepsHeard = true;
        engine.State.Flags["ai_temple_magic_bonus"] = "true";
        engine.State.Player.RecalcStats();
        _message = "Приближавате се и чувате странни компютърни звуци в съзнанието си за секунда. След това се чувствате по-умни.";
        EnsureOptions(engine);
    }

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
            foreach (var opt in _options)
            {
                Console.WriteLine($" {opt.Id}. {opt.Text}");
            }
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
            if (!InputHandler.Handle(input, _options, out Option selectedOption))
            {
                selectedOption.OnSelect?.Invoke(engine);
            }
        }
        else
        {
            _done = true;
        }
    }
}
