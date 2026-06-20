using Harduni.Core;
using System;

namespace Harduni.Locations;

public class DebugLocation : Location
{
    private string _message = "";

    public DebugLocation(World world) : base(world, "Дебъг локация", "Добре дошли в тайната дебъг зона на Кордор.")
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        _message = "";
        BuildOptions(engine);
    }

    private void BuildOptions(GameEngine engine)
    {
        Options.Clear();
        int id = 1;

        Options.Add(new Option(id++, "Качване на ниво (Level Up)", "Повишава вашето ниво с 1.", (eng) => 
        {
            var p = eng.State.Player;
            p.Level++;
            p.RecalculateBaseStats();
            p.Hp = p.MaxHp;
            p.Mp = p.MaxMp;
            _message = $"Нивото е повишено на {p.Level}!";
            BuildOptions(eng);
        }));

        Options.Add(new Option(id++, "Сваляне на ниво (Level Down)", "Намалява вашето ниво с 1.", (eng) => 
        {
            var p = eng.State.Player;
            if (p.Level > 1)
            {
                p.Level--;
                p.RecalculateBaseStats();
                p.Hp = p.MaxHp;
                p.Mp = p.MaxMp;
                _message = $"Нивото е намалено на {p.Level}!";
            }
            else
            {
                _message = "Не можете да слизате под ниво 1!";
            }
            BuildOptions(eng);
        }));

        Options.Add(new Option(id++, "Тест Анимация Око (Debug)", "Тества анимацията на окото.", (eng) => 
        {
            var animPanel = new Harduni.Events.BlinkingEyeAnimationPanel((e) => e.ChangeRootPanel(this));
            eng.ChangeRootPanel(animPanel);
        }));

        Options.Add(new Option(id++, "Назад към Кордор", "Връща ви в предходната локация.", (eng) => 
        {
            eng.ChangeRootPanel(World.Kordor);
        }));
    }

    public override void Render(GameEngine engine)
    {
        VConsole.WriteLine($"=== {Name} ===");
        VConsole.WriteLine(Description);
        VConsole.WriteLine($"Текущо ниво на играча: {engine.State.Player.Level}");
        
        if (!string.IsNullOrEmpty(_message))
        {
            VConsole.WriteLine($"\n[ {_message} ]");
        }

        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in Options)
        {
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public override void ProcessInput(string input, GameEngine engine)
    {
        _message = "";
        if (!InputHandler.Handle(input, Options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
