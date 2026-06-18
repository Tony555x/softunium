using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Locations;

namespace Harduni.Events;

public class CleanRoomLaptopEvent : IPanel
{
    private enum State { Initial, LaptopRead, BurntLaptop }
    private State _state = State.Initial;
    private State _lastBuiltState = (State)(-1);
    private readonly List<Option> _options = new();
    private bool _healedThisTime = false;

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("laptop_skill_slot_bonus"))
        {
            _state = State.BurntLaptop;
            if (!_healedThisTime)
            {
                engine.State.Player.Heal(20);
                _healedThisTime = true;
            }
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
        if (_state == State.Initial)
        {
            _options.Add(new Option(1, "Погледни", "Погледнете какво има на екрана на лаптопа.", (eng) => StartAnimation(eng)));
        }
        else if (_state == State.LaptopRead)
        {
            _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => eng.State.DungeonData.IsEventActive = false));
        }
        else if (_state == State.BurntLaptop)
        {
            _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => eng.State.DungeonData.IsEventActive = false));
        }

        _lastBuiltState = _state;
    }

    private void StartAnimation(GameEngine engine)
    {
        var anim = new BlinkingEyeAnimationPanel((eng) => 
        {
            _state = State.LaptopRead;
            eng.State.Flags["laptop_skill_slot_bonus"] = "true";
            eng.State.Player.RecalcStats();
            // Restore current event panel as the active panel
            eng.ChangeRootPanel(eng.State.DungeonData.Rooms[eng.State.DungeonData.CurrentRoomIndex].EventInstance);
        });
        engine.ChangeRootPanel(anim);
    }

    public void Render(GameEngine engine)
    {
        EnsureOptions(engine);

        if (_state == State.Initial)
        {
            VConsole.WriteLine("=== ЧИСТА СТАЯ ===");
            VConsole.WriteLine("Намирате странна структура от чинове. Когато влизате, усещате че въздухът вътре е чист! Също, има отворен лаптоп на пода?");
        }
        else if (_state == State.LaptopRead)
        {
            VConsole.WriteLine("=== ЛАПТОП ===");
            VConsole.WriteLine("Успявате да прочетете само една дума.");
            VConsole.WriteLine("\n\"УТАЙКА\"");
            VConsole.WriteLine("\nЛаптопът веднага се самозапалва след това.");
            VConsole.WriteLine("Не знаете какво е това, но думата събужда неясни спомени и ярост в вас.");
            VConsole.WriteLine("\n(+1 Слот за умения!)");
        }
        else if (_state == State.BurntLaptop)
        {
            VConsole.WriteLine("=== ИЗГОРЯЛ ЛАПТОП ===");
            VConsole.WriteLine("Намирате стаята с изгорелия лаптоп. Тук е чисто и тихо, което ви позволява да си починете за момент.");
            VConsole.WriteLine("(Възстановихте 20 Живот.)");
        }

        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
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
