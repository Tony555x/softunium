using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Events;

public class LeadershipDoorEvent : IPanel
{
    private enum State { Initial, Opened, StaticFlash, EyeRevealed, DoorTaken, LockerMissing, StatsShown }
    private State _state = State.Initial;
    private State _lastBuiltState = (State)(-1);
    private readonly List<Option> _options = new();
    
    private float _staticTimer = 0.05f;
    private static readonly Random _rand = new();

    public void Update(float deltaTime, GameEngine engine)
    {
        if (_state == State.StaticFlash)
        {
            _staticTimer -= deltaTime;
            if (_staticTimer <= 0f)
            {
                _state = State.EyeRevealed;
                _staticTimer = 0.05f; // reset just in case
                VConsole.Clear();
            }
        }
    }

    public void OnOpen(GameEngine engine)
    {
        if (engine.State.Flags.ContainsKey("progress_door_weight_bonus"))
        {
            _state = State.LockerMissing;
        }
        EnsureOptions(engine);
    }

    private void EnsureOptions(GameEngine engine)
    {
        if (_state == _lastBuiltState && _state != State.StaticFlash) return;

        _options.Clear();
        if (_state == State.Initial)
        {
            _options.Add(new Option(1, "Вземи ключа", "Вземи ключа.", (eng) => TakeDoor(eng)));
        }
        else if (_state == State.Opened)
        {
            _options.Add(new Option(1, "Тръгни си", "Остави го.", (eng) => _state = State.StaticFlash));
        }
        else if (_state == State.EyeRevealed)
        {
            _options.Add(new Option(1, "Погледни вътре", "Погледни вътре.", (eng) => StartAnimation(eng)));
        }
        else if (_state == State.DoorTaken)
        {
            _options.Add(new Option(1, "Вземи вратата", "Вземи я.", (eng) => TakeDoor(eng)));
            _options.Add(new Option(2, "Вземи вратата", "Вземи я.", (eng) => TakeDoor(eng)));
        }
        else if (_state == State.StatsShown)
        {
            _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => ReturnFromEvent(eng)));
        }
        else if (_state == State.LockerMissing)
        {
            _options.Add(new Option(1, "Продължи", "Продължете напред.", (eng) => ReturnFromEvent(eng)));
        }

        _lastBuiltState = _state;
    }

    private void StartAnimation(GameEngine engine)
    {
        /*
        var prevPanel = engine.CurrentPanel;
        var anim = new DoorOpeningAnimationPanel((eng) => 
        {
            _state = State.DoorTaken;
            eng.ChangeRootPanel(prevPanel);
        });
        engine.ChangeRootPanel(anim);
        */
        
        _state = State.DoorTaken;
    }

    private void TakeDoor(GameEngine engine)
    {
        engine.State.Flags["progress_door_weight_bonus"] = "true";
        engine.State.Player.RecalcStats();
        _state = State.StatsShown;
        EnsureOptions(engine);
    }

    private void ReturnFromEvent(GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
        if (engine.CurrentPanel == this)
        {
            engine.ChangeRootPanel(engine.State.World.DebugLocation);
        }
    }

    public void Render(GameEngine engine)
    {
        EnsureOptions(engine);

        if (_state == State.StaticFlash)
        {
            int width = 80;
            int height = 24;
            try
            {
                width = VConsole.WindowWidth;
                height = VConsole.WindowHeight;
            }
            catch { }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    sb.Append(_rand.Next(2) == 0 ? '#' : ' ');
                }
                sb.AppendLine();
            }
            VConsole.SetCursorPosition(0, 0);
            VConsole.Write(sb.ToString());
            return; // Skip drawing normal UI
        }

        if (_state == State.Initial)
        {
            VConsole.WriteLine("Минавайки покрай една от панелките виждате шкафчета до нея. Едно от шкафчетата има ключ вътре. Това може да е полезно?");
        }
        else if (_state == State.Opened)
        {
            VConsole.WriteLine("Няма нищо вътре... И вътре е много тъмно..?");
        }
        else if (_state == State.EyeRevealed)
        {
            VConsole.WriteLine("Няма нищо вътре... И вътре е много тъмно..?");
        }
        else if (_state == State.DoorTaken)
        {
            VConsole.WriteLine("...");
        }
        else if (_state == State.StatsShown)
        {
            VConsole.WriteLine("Чрез ключа можеш да 'отвориш' шкафчето. +2 Макс Тежест на инвентара.");
        }
        else if (_state == State.LockerMissing)
        {
            VConsole.WriteLine("В стената от шкафчета липсва едно..");
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
        if (_state == State.StaticFlash) return;

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
            EnsureOptions(engine);
        }
    }
}
