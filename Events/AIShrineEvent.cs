using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Events;

public class AIShrineEvent : IPanel
{
    private enum EventState { Initial, Repeating, BeepsHeard }
    private EventState _state = EventState.Initial;
    private string _message = "";
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("ai_shrine_magic_bonus"))
        {
            _state = EventState.Repeating;
        }

        else
        {
            _state = EventState.Initial;
        }
        EnsureOptions(engine);
    }

    private void EnsureOptions(GameEngine engine)
    {
        _options.Clear();
        
        switch (_state)
        {
            case EventState.Initial:
                _message = "Намирате странен олтар, заобиколен от кръг от вода.\nУсещате лека магическа аура наоколо.";
                _options.Add(new Option(1, "Приближи се", "Приближете се към олтара.", (eng) => Approach(eng)));
                break;

            case EventState.Repeating:
                _message = "Виждате странният олтар отново. Водата все още гори! Но не усещате нищо странно.";
                break;

            case EventState.BeepsHeard:
                _message = "Когато стъпвате наблизо, водата се запалва! Едновременно с това чувате странни компютърни звуци в съзнанието си.\nЧувствате се по-умни. (Получихте +4 Магия перманентно!)";
                break;
        }
    }

    private void Approach(GameEngine engine)
    {
        _state = EventState.BeepsHeard;
        engine.State.Flags["ai_shrine_magic_bonus"] = "true";
        engine.State.Player.RecalcStats();
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("=== Странен Олтар ===");
        Console.WriteLine(_message);


        if (_state == EventState.Initial)
        {
            Console.WriteLine("\nВъзможни действия:");
            foreach (var opt in _options)
            {
                Console.WriteLine($" {opt.Id}. {opt.Text}");
            }
        }
        else
        {
            Console.WriteLine("\n[Натиснете Enter за продължаване]");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (_state == EventState.Initial)
        {
            if (!InputHandler.Handle(input, _options, out Option selectedOption))
            {
                selectedOption.OnSelect?.Invoke(engine);
            }
        }
        else
        {
            engine.State.DungeonData.IsEventActive = false;
        }
    }
}
