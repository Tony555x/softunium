using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Locations;

namespace Harduni.Events;

public class TempoUnlockEvent : IPanel
{
    private enum State
    {
        Initial,
        AlexIntro,
        AlexQuestion,
        AlexAnswerYes,
        AlexAnswerNo,
        AlexWhat,
        AlexChallenge,
        Fighting,
        FightWon,
        AlexDisappear,
        TempoUnlocked,
        QuietRoom
    }

    private State _state = State.Initial;
    private State _lastBuiltState = (State)(-1);
    private readonly List<Option> _options = new();
    private readonly List<string> _unlockMessages = new();

    public void Update(float deltaTime, GameEngine engine)
    {
        if (_state == State.Fighting)
        {
            var battleData = engine.State.BattleData;
            if (battleData.IsFinished && battleData.Enemies.Any(e => e is AlexTempoEventEnemy && e.Hp <= 0) && engine.State.Player.Hp > 0)
            {
                _state = State.FightWon;
                EnsureOptions(engine);
            }
        }
    }

    private void EnsureOptions(GameEngine engine)
    {
        if (_state == _lastBuiltState) return;

        _options.Clear();
        if (_state == State.Initial)
        {
            _options.Add(new Option(1, "Говори", "Опитайте се да говорите с Алекс.", (eng) => _state = State.AlexIntro));
        }
        else if (_state == State.AlexIntro)
        {
            _options.Add(new Option(1, "Здрасти?", "Кажете здрасти на Алекс.", (eng) => _state = State.AlexQuestion));
        }
        else if (_state == State.AlexQuestion)
        {
            _options.Add(new Option(1, "Да?", "Отговорете утвърдително.", (eng) => _state = State.AlexAnswerYes));
            _options.Add(new Option(2, "Не?", "Отговорете отрицателно.", (eng) => _state = State.AlexAnswerNo));
        }
        else if (_state == State.AlexAnswerYes || _state == State.AlexAnswerNo)
        {
            _options.Add(new Option(1, "Какво", "Попитайте защо.", (eng) => _state = State.AlexWhat));
        }
        else if (_state == State.AlexWhat)
        {
            _options.Add(new Option(1, "Ок?", "Приемете думите му.", (eng) => _state = State.AlexChallenge));
        }
        else if (_state == State.AlexChallenge)
        {
            _options.Add(new Option(1, "Бий се", "Влезте в битка с Алекс.", (eng) => StartFight(eng)));
        }
        else if (_state == State.FightWon)
        {
            _options.Add(new Option(1, "...какво?", "Попитайте за неразбираемото име.", (eng) => _state = State.AlexDisappear));
        }
        else if (_state == State.AlexDisappear)
        {
            _options.Add(new Option(1, "...", "Останете без думи.", (eng) =>
            {
                _state = State.TempoUnlocked;
                eng.State.Flags["tempo_unlocked"] = "true";
                _unlockMessages.Clear();
                eng.State.Player.CheckAndAddTempoSkills(_unlockMessages);
            }));
        }
        else if (_state == State.TempoUnlocked)
        {
            _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => FinishEvent(eng)));
        }
        else if (_state == State.QuietRoom)
        {
            _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => FinishEvent(eng)));
        }

        _lastBuiltState = _state;
    }

    private void StartFight(GameEngine engine)
    {
        _state = State.Fighting;
        BattleManager.StartBattle(engine, new List<Enemy> { new AlexTempoEventEnemy() }, engine.State.World.ProgressDungeon);
    }

    private void FinishEvent(GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("tempo_unlocked"))
        {
            _state = State.QuietRoom;
        }
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        EnsureOptions(engine);

        if (_state == State.Initial)
        {
            Console.WriteLine("Намираш Алекс да гледа в далечината...");
        }
        else if (_state == State.AlexIntro)
        {
            Console.WriteLine("Алекс: \"...\"");
        }
        else if (_state == State.AlexQuestion)
        {
            Console.WriteLine("Алекс: \"...Ходиш ли?\"");
        }
        else if (_state == State.AlexAnswerYes)
        {
            Console.WriteLine("Алекс те поглежда разочаровано.");
            Console.WriteLine("Алекс: \"Не достатъчно...\"");
        }
        else if (_state == State.AlexAnswerNo)
        {
            Console.WriteLine("Алекс те поглежда разочаровано.");
            Console.WriteLine("Алекс: \"Това е видимо...\"");
        }
        else if (_state == State.AlexWhat)
        {
            Console.WriteLine("Алекс: \"Но може би имаш потенциал? Да преминеш отвъд границите на БАСТУН.\"");
        }
        else if (_state == State.AlexChallenge)
        {
            Console.WriteLine("Алекс: \"Но трябва да го докажеш!\".");
            Console.WriteLine("Алекс те атакува!");
        }
        else if (_state == State.FightWon)
        {
            Console.WriteLine("Алекс: \"...Добре. Може би ти ще си този който ще победи ██?██?███\"");
            Console.WriteLine("Получаваш силно главоболие когато чуваш името!");
        }
        else if (_state == State.AlexDisappear)
        {
            Console.WriteLine("Алекс се обръща без да те изчаква. С първата стъпка кято взима, той изчезва от стаята!");
        }
        else if (_state == State.TempoUnlocked)
        {
            Console.WriteLine("Какво се случи..?");
            Console.WriteLine("Ти отключи ТЕМПО!");
            if (_unlockMessages.Count > 0)
            {
                Console.WriteLine();
                foreach (var msg in _unlockMessages)
                {
                    Console.WriteLine(msg);
                }
            }
            Console.WriteLine("\nПолучаваш едно темпо всеки път когато използваш атака или нормално умение.");
            Console.WriteLine("Използвай темпо за да активираш специални умения!");
            Console.WriteLine("Екипирай си специалните умения от кордор.");
            Console.WriteLine("Вече можеш да виждаш степента си...");
            Console.WriteLine("Степен: БАСТУН");
        }
        else if (_state == State.QuietRoom)
        {
            Console.WriteLine("Стаята е напълно тиха. Алекс вече го няма тук.");
            Console.WriteLine("Чуваш единствено собственото си дишане сред празното пространство.");
        }

        Console.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        EnsureOptions(engine);

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
            EnsureOptions(engine);
        }
    }
}
