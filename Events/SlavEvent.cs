using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Relics;

namespace Harduni.Events;

public class SlavEvent : IPanel
{
    private enum SlavState { Initial, Talking1, Talking2, Talking3, Done, Repeated, Repeated2, Repeated3 }
    private SlavState _state = SlavState.Initial;
    private SlavState _lastBuiltState = (SlavState)(-1);
    private List<Option> _options = new();
    private string _message = "";
    private readonly SlavEnemy _slavEnemy = new();

    public void Update(float deltaTime, GameEngine engine) 
    {
        // Check if player won the fight
        var battleData = engine.State.BattleData;
        if (battleData.IsFinished && battleData.Enemies.Any(e => e is SlavEnemy && e.Hp <= 0) && engine.State.Player.Hp > 0)
        {
            if (!engine.State.Flags.ContainsKey("SlavDefeated"))
            {
                FinishFight(engine);
            }
        }
    }

    private void FinishFight(GameEngine engine)
    {
        engine.State.Flags["slav_defeated"] = "true";
        _message = "Слав: how the fuck";
        var data = engine.State.DungeonData;
        data.IsEventActive = false;
    }

    private void EnsureOptions(GameEngine engine)
    {
        if (_state == _lastBuiltState) return;

        _options.Clear();
        if (_state == SlavState.Initial)
        {
            _options.Add(new Option(1, "Говори", "Опитайте се да говорите със Слав.", (eng) => 
            {
                if (eng.State.Flags.ContainsKey("WisdomRoomsUnlocked"))
                {
                    _state = SlavState.Repeated;
                }
                else
                {
                    _state = SlavState.Talking1;
                }
            }));
        }
        else if (_state == SlavState.Talking1)
        {
            _options.Add(new Option(1, "Здрасти", "Кажете здрасти на Слав.", (eng) => _state = SlavState.Talking2));
        }
        else if (_state == SlavState.Talking2)
        {
            _options.Add(new Option(1, "А къде?", "Попитайте Слав къде другаде да отидете.", (eng) => _state = SlavState.Talking3));
        }
        else if (_state == SlavState.Talking3)
        {
            _options.Add(new Option(1, "Ок", "Приемете информацията и продължете.", (eng) => FinishTalking(eng)));
        }
        else if (_state == SlavState.Repeated)
        {
            _options.Add(new Option(1, "Здрасти отново", "Поздравете Слав отново.", (eng) => _state = SlavState.Repeated2));
        }
        else if (_state == SlavState.Repeated2)
        {
            if (engine.State.Flags.ContainsKey("relics_unlocked") && !engine.State.Flags.ContainsKey("obtained_flash_drive"))
            {
                _options.Add(new Option(1, "Вземи 10 TB флашка", "Вземете реликвата от Слав.", (eng) => 
                {
                    eng.State.Flags["obtained_flash_drive"] = "true";
                    var p = eng.State.Player;
                    p.Relics.Add(new FlashDrive());
                    _message = "Получихте реликва: 10 TB флашка!";
                    _state = SlavState.Repeated3;
                }));
            }
            else
            {
                _options.Add(new Option(1, "Тръгни си", "Продължете пътя си.", (eng) => Leave(eng)));
            }
        }
        else if (_state == SlavState.Repeated3)
        {
            _options.Add(new Option(1, "Тръгни си", "Продължете пътя си.", (eng) => Leave(eng)));
        }
        _lastBuiltState = _state;
    }

    private void Leave(GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }

    private void FinishTalking(GameEngine engine)
    {
        engine.State.Flags["WisdomRoomsUnlocked"] = "true";
        
        var data = engine.State.DungeonData;
        data.IsEventActive = false;
    }

    public void OnOpen(GameEngine engine)
    {
        _state = SlavState.Initial;
        _lastBuiltState = (SlavState)(-1);
        _message = "";
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        if (!string.IsNullOrEmpty(_message))
        {
            VConsole.WriteLine($"\n{_message}");
            return;
        }

        if (_state == SlavState.Initial)
        {
            VConsole.WriteLine("Виждате Слав да стои спокойно в края на залата.");
        }
        else if (_state == SlavState.Talking1)
        {
            VConsole.WriteLine("Слав: Здрасти.");
        }
        else if (_state == SlavState.Talking2)
        {
            VConsole.WriteLine("Слав: Защо си тук?");
        }
        else if (_state == SlavState.Talking3)
        {
            VConsole.WriteLine("Слав: Там.");
            VConsole.WriteLine("Отключени са стаите: 'Стая Прогрес', 'Стая Тимуърк' и 'Стая Интегрити'.");
        }
        else if (_state == SlavState.Repeated)
        {
            VConsole.WriteLine("Слав: Ей отново.");
        }
        else if (_state == SlavState.Repeated2)
        {
            if (engine.State.Flags.ContainsKey("relics_unlocked") && !engine.State.Flags.ContainsKey("obtained_flash_drive"))
            {
                VConsole.WriteLine("Слав: А, отключил си реликви. Вземи тази 10 TB флашка.");
            }
            else
            {
                VConsole.WriteLine("Слав: Здрасти отново. Няма какво повече да ти кажа.");
            }
        }
        else if (_state == SlavState.Repeated3)
        {
            VConsole.WriteLine("Слав стои спокойно в края на залата.");
        }

        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (!string.IsNullOrEmpty(_message))
        {
            _message = "";
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
            EnsureOptions(engine); // Update options if state changed
        }
    }
}
