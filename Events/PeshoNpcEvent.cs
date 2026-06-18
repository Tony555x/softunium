using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Relics;

namespace Harduni.Events;

public class PeshoNpcEvent : IPanel
{
    private enum State
    {
        Initial,
        Talking1,
        Talking2,
        Talking3,
        Talking4,
        Done,
        AftermathInitial,
        AftermathTalking
    }

    private State _state = State.Initial;
    private State _lastBuiltState = (State)(-1);
    private readonly List<Option> _options = new();
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    private void EnsureOptions(GameEngine engine)
    {
        if (_state == _lastBuiltState) return;
        _options.Clear();

        if (_state == State.Initial)
        {
            _options.Add(new Option(1, "Говори", "Говорете с Пешо.", (eng) => _state = State.Talking1));
        }
        else if (_state == State.Talking1)
        {
            _options.Add(new Option(1, "Какво е това?", "Попитайте го за левитиращите предмети.", (eng) => _state = State.Talking2));
        }
        else if (_state == State.Talking2)
        {
            _options.Add(new Option(1, "??", "Погледнете обекта изненадано.", (eng) => _state = State.Talking3));
        }
        else if (_state == State.Talking3)
        {
            _options.Add(new Option(1, "Как търгуваш в тему", "Попитайте го как работи схемата.", (eng) => 
            {
                _state = State.Talking4;
                // Give relic and unlock flag here to ensure it's recorded
                if (!eng.State.Flags.ContainsKey("relics_unlocked"))
                {
                    eng.State.Flags["relics_unlocked"] = "true";
                    var p = eng.State.Player;
                    if (!p.Relics.Exists(r => r is BubbleGun))
                    {
                        p.Relics.Add(new BubbleGun());
                        _message = "Получихте реликва: Пистолет за балончета!";
                    }
                }
            }));
        }
        else if (_state == State.Talking4)
        {
            _options.Add(new Option(1, "Тръгни си", "Продължете напред.", (eng) => FinishEvent(eng)));
        }
        else if (_state == State.AftermathInitial)
        {
            _options.Add(new Option(1, "Говори", "Говорете с Пешо.", (eng) => _state = State.AftermathTalking));
        }
        else if (_state == State.AftermathTalking)
        {
            _options.Add(new Option(1, "Тръгни си", "Продължете напред.", (eng) => FinishEvent(eng)));
        }

        _lastBuiltState = _state;
    }

    private void FinishEvent(GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("relics_unlocked"))
        {
            _state = State.AftermathInitial;
        }
        else
        {
            _state = State.Initial;
        }
        _lastBuiltState = (State)(-1);
        _message = "";
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        EnsureOptions(engine);

        if (!string.IsNullOrEmpty(_message))
        {
            VConsole.WriteLine($"\n{_message}");
            _message = "";
        }

        if (_state == State.Initial)
        {
            VConsole.WriteLine("Намирате Пешо с случайни боклуци левитиращи около него?!");
        }
        else if (_state == State.Talking1)
        {
            VConsole.WriteLine("Пешо: Здравей!");
        }
        else if (_state == State.Talking2)
        {
            VConsole.WriteLine("Пешо: Взех ги от ТЕМУ. Ето дръж това");
            VConsole.WriteLine("Той вади странен обект от въздуха и ти го хвърля");
        }
        else if (_state == State.Talking3)
        {
            VConsole.WriteLine("Пешо: Това е пистолет за балончета! Струваше някво 10 стотинки преди да го изтъргувам да 2 и половина, имам 10 от тези");
        }
        else if (_state == State.Talking4)
        {
            VConsole.WriteLine("Пешо: айде");
            VConsole.WriteLine("Обурудвай се с реликви от кордор.");
            VConsole.WriteLine("Те имат пасивни умения.");
            VConsole.WriteLine("Можеш да получаваш нови реликви от всякакви места, главно от това да говориш с герои.");
        }
        else if (_state == State.AftermathInitial)
        {
            VConsole.WriteLine("Намирате Пешо да разглежда нови оферти в ТЕМУ на 4 ТЕМУ телефона едновременно.");
        }
        else if (_state == State.AftermathTalking)
        {
            VConsole.WriteLine("Изглежда зает... Фокусирал се е на играчка на странно куче с резачка на него.");
        }

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
            EnsureOptions(engine);
        }
    }
}
