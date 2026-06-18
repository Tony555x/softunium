using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Events;

public class AndrewTateShrineEvent : IPanel
{
    private enum EventState { Initial, Repeating, BeepsHeard }
    private EventState _state = EventState.Initial;
    private string _message = "";
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("andrew_tate_atk_bonus"))
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
                _message = "Намирате странен олтар, заобиколен от кръг от вода.\n Чувствате странна агресивна аура наоколо, поканваща ви да счупите нещо.";
                _options.Add(new Option(1, "Приближи се", "Приближете се до олтара", (eng) => Approach(eng)));
                break;

            case EventState.Repeating:
                _message = "Виждате олтара отново.";
                break;

            case EventState.BeepsHeard:
                _message = "Когато се покланяте, чувате глас в главата си: 'What color is your Bugatti?'.\nИзведнъж се чувствате много по-силни. (Получихте +4 Атака перманентно!)";
                break;
        }
    }

    private void Approach(GameEngine engine)
    {
        _state = EventState.BeepsHeard;
        engine.State.Flags["andrew_tate_atk_bonus"] = "true";
        engine.State.Player.RecalcStats();
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== Странен Олтар ===");
        VConsole.WriteLine(_message);

        if (_state == EventState.Initial)
        {
            VConsole.WriteLine("\nВъзможни действия:");
            foreach (var opt in _options)
            {
                VConsole.WriteLine($" {opt.Id}. {opt.Text}");
            }
        }
        else
        {
            VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
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
