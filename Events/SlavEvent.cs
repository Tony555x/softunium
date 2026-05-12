using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Enemies;

namespace Harduni.Events;

public class SlavEvent : IPanel
{
    private enum SlavState { Initial, Talking1, Talking2, Talking3, Done }
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
            _options.Add(new Option(1, "Говори", "Опитайте се да говорите със Слав.", (eng) => _state = SlavState.Talking1));
            _options.Add(new Option(2, "Бий се", "Предизвикайте Слав на двубой.", (eng) => StartFight(eng)));
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
        _lastBuiltState = _state;
    }

    private void StartFight(GameEngine engine)
    {
        BattleManager.StartBattle(engine, new List<Enemy> { _slavEnemy }, engine.State.World.WisdomDungeon);
    }

    private void FinishTalking(GameEngine engine)
    {
        engine.State.Flags["WisdomRoomsUnlocked"] = "true";
        
        var data = engine.State.DungeonData;
        data.IsEventActive = false;
    }

    public void Render(GameEngine engine)
    {
        if (!string.IsNullOrEmpty(_message))
        {
            Console.WriteLine($"\n{_message}");
            return;
        }

        EnsureOptions(engine);
        //Console.WriteLine("<=- СЛАВ -=>");
        if (_state == SlavState.Initial)
        {
            Console.WriteLine("Виждате Слав да стои спокойно в края на залата.");
        }
        else if (_state == SlavState.Talking1)
        {
            Console.WriteLine("Слав: Здрасти.");
        }
        else if (_state == SlavState.Talking2)
        {
            Console.WriteLine("Слав: Защо си тук?");
        }
        else if (_state == SlavState.Talking3)
        {
            Console.WriteLine("Слав: Там.");
            Console.WriteLine("Отключени са стаите: 'Стая Прогрес', 'Стая Тимуърк' и 'Стая Интегрити'.");
        }

        Console.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (!string.IsNullOrEmpty(_message))
        {
            _message = "";
            return;
        }

        EnsureOptions(engine);
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
