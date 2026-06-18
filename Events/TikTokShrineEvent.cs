using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Events;

public class TikTokShrineEvent : IPanel
{
    private enum EventState { Initial, Repeating, BeepsHeard }
    private EventState _state = EventState.Initial;
    private string _message = "";
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("tiktok_shrine_def_bonus"))
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
                _message = "Намирате странен олтар, заобиколен от кръг от вода.\nУсещате присъствието на картини и звуци наоколо.";
                _options.Add(new Option(1, "Приближи се", "Приближете се към олтара.", (eng) => Approach(eng)));
                break;

            case EventState.Repeating:
                _message = "Виждате странният олтар. Водата около него е черна като мастило.";
                break;

            case EventState.BeepsHeard:
                _message = "Когато се приближите, водата около олтара става черна като мастило!\nВ съзнанието ви нахлува кратка какофония от неразбираеми изображения и звуци.\nЧувствате ума си по-затворен, но защитен. (Получихте +4 Защита перманентно!)";
                break;
        }
    }

    private void Approach(GameEngine engine)
    {
        _state = EventState.BeepsHeard;
        engine.State.Flags["tiktok_shrine_def_bonus"] = "true";
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
