using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Skills;

namespace Harduni.Events;

public class MirrorEvent : IPanel
{
    private enum State { Initial, Normal, CatchUp, Imitated, GiveUp }
    private State _state = State.Initial;
    private State _lastBuiltState = (State)(-1);
    private readonly List<Option> _options = new();
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        var p = engine.State.Player;
        if (p.Skills.Any(s => s.Name == "Походка"))
        {
            _state = State.Normal;
        }
        else
        {
            _state = State.Initial;
        }
        EnsureOptions(engine);
    }

    private void EnsureOptions(GameEngine engine)
    {
        if (_state == _lastBuiltState) return;

        _options.Clear();
        var p = engine.State.Player;

        if (_state == State.Initial)
        {
            _options.Add(new Option(1, "Настигни го", "Опитай се да настигнеш отражението си.", (eng) => _state = State.CatchUp));
        }
        else if (_state == State.Normal)
        {
            _options.Add(new Option(1, "Продължи", "Продължи напред.", (eng) => ReturnFromEvent(eng)));
        }
        else if (_state == State.CatchUp)
        {
            bool canImitate = p.Mp >= 20;
            _options.Add(new Option(1, "Имитирай го (20 Айрян)", "Опитай се да имитираш походката му.", (eng) => 
            {
                if (canImitate)
                {
                    p.Mp -= 20;
                    var skill = new Gait();
                    p.Skills.Add(skill);
                    if (p.EquippedSkills.Count < p.MaxSkillSlots)
                    {
                        p.EquippedSkills.Add(skill);
                        p.RecalcStats();
                    }
                    _state = State.Imitated;
                }
                else
                {
                    _message = "Нямате достатъчно Айрян!";
                }
            }, !canImitate, "Имитирай"));

            _options.Add(new Option(2, "Откажи се", "Откажи се и продължи.", (eng) => _state = State.GiveUp));
        }
        else if (_state == State.Imitated)
        {
            _options.Add(new Option(1, "Продължи", "Продължи напред.", (eng) => ReturnFromEvent(eng)));
        }
        else if (_state == State.GiveUp)
        {
            _options.Add(new Option(1, "Продължи", "Продължи напред.", (eng) => ReturnFromEvent(eng)));
        }

        _lastBuiltState = _state;
    }

    private void ReturnFromEvent(GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }

    public void Render(GameEngine engine)
    {
        EnsureOptions(engine);
        VConsole.WriteLine("=== ОГЛЕДАЛО ===");

        if (!string.IsNullOrEmpty(_message))
        {
            VConsole.WriteLine(_message);
            VConsole.WriteLine();
        }

        if (_state == State.Normal)
        {
            VConsole.WriteLine("Намираш огледалото отново, но отражението ти е напълно нормално.");
        }
        else if (_state == State.Initial)
        {
            VConsole.WriteLine("Докато пътуваш намираш голямо огледало което покрива цялата лява стена.");
            VConsole.WriteLine("Докато ходиш осъзнаваш, че отражението ти е по-бързо от теб.");
        }
        else if (_state == State.CatchUp)
        {
            VConsole.WriteLine("Ходиш по-бързо, но не можеш да го настигнеш! Начинът то който ходи е по-мощен от твоят.");
        }
        else if (_state == State.Imitated)
        {
            VConsole.WriteLine("Опитваш се да ходиш както отражението ти ходи. Трудно е, но ставаш по-бърз!");
            VConsole.WriteLine("Но точно преди да настигнеш отражението си, огледалото свършва.");
            VConsole.WriteLine("Когато се върнеш отражението ти е напълно нормално.\n");
            VConsole.WriteLine("Ти научи ново умение: Походка");
            if (engine.State.Player.EquippedSkills.Count >= engine.State.Player.MaxSkillSlots)
            {
                VConsole.WriteLine("(Нямате свободни слотове, умението е добавено в инвентара ви.)");
            }
        }
        else if (_state == State.GiveUp)
        {
            VConsole.WriteLine("Изглежда твърде трудно за да го имитираш. Отражението ти изчезва в ръба на огледалото.");
        }

        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            if (opt.IsDisabled) VConsole.ForegroundColor = ConsoleColor.DarkGray;
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
            VConsole.ResetColor();
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        EnsureOptions(engine);
        _message = "";

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            if (!selectedOption.IsDisabled)
            {
                selectedOption.OnSelect?.Invoke(engine);
                EnsureOptions(engine);
            }
        }
    }
}
