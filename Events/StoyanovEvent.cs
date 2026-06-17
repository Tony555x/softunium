using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Items;
using Harduni.Relics;


namespace Harduni.Events;

public class StoyanovEvent : IPanel
{
    private enum ShopState { Initial, Talking1, Talking2, Talking3, Done, Repeated, Repeated2, Repeated3 }
    private ShopState _state = ShopState.Initial;
    private ShopState _lastBuiltState = (ShopState)(-1);
    private List<Option> _options = new();
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    private void EnsureOptions(GameEngine engine)
    {
        if (_state == _lastBuiltState) return;
        _options.Clear();

        if (_state == ShopState.Initial)
        {
            _options.Add(new Option(1, "Говори", "Говорете със Стоянов.", (eng) => 
            {
                if (eng.State.Flags.ContainsKey("shop_unlocked"))
                {
                    _state = ShopState.Repeated;
                }
                else
                {
                    _state = ShopState.Talking1;
                }
            }));
        }
        else if (_state == ShopState.Talking1)
        {
            _options.Add(new Option(1, "Здрасти?", "Отговорете на поздрава.", (eng) => _state = ShopState.Talking2));
        }
        else if (_state == ShopState.Talking2)
        {
            _options.Add(new Option(1, "Ок", "Съгласете се.", (eng) => _state = ShopState.Talking3));
        }
        else if (_state == ShopState.Talking3)
        {
            _options.Add(new Option(1, "Продължи", "Към лафката!", (eng) => FinishTalking(eng)));
        }
        else if (_state == ShopState.Repeated)
        {
            _options.Add(new Option(1, "Не", "Откажете.", (eng) => _state = ShopState.Repeated2));
        }
        else if (_state == ShopState.Repeated2)
        {
            if (engine.State.Flags.ContainsKey("relics_unlocked") && !engine.State.Flags.ContainsKey("obtained_deodorant"))
            {
                _options.Add(new Option(1, "Вземи Дезодорант", "Вземете реликвата.", (eng) => 
                {
                    eng.State.Flags["obtained_deodorant"] = "true";
                    var p = eng.State.Player;
                    p.Relics.Add(new Deodorant());
                    _message = "Получихте реликва: Дезодорант!";
                    _state = ShopState.Repeated3;
                }));
            }
            else
            {
                _options.Add(new Option(1, "Тръгни си", "Продължете пътя си.", (eng) => Leave(eng)));
            }
        }
        else if (_state == ShopState.Repeated3)
        {
            _options.Add(new Option(1, "Тръгни си", "Продължете пътя си.", (eng) => Leave(eng)));
        }

        _lastBuiltState = _state;
    }

    private void FinishTalking(GameEngine engine)
    {
        engine.State.Flags["shop_unlocked"] = "true";
        engine.State.DungeonData.IsEventActive = false;
        engine.ChangeRootPanel(engine.State.World.ShopPanel);
    }

    private void Leave(GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }

    public void OnOpen(GameEngine engine)
    {
        _state = ShopState.Initial;
        _lastBuiltState = (ShopState)(-1);
        EnsureOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        if (!string.IsNullOrEmpty(_message))
        {
            Console.WriteLine($"\n{_message}");
            _message = "";
        }

        if (_state == ShopState.Initial)
        {
            Console.WriteLine("Намираш Стоянов да яде пица в стаята на един от чиновете на улицата.");
        }
        else if (_state == ShopState.Talking1)
        {
            Console.WriteLine("Стоянов: \"Ейййй здрасти!\"");
        }
        else if (_state == ShopState.Talking2)
        {
            Console.WriteLine("Стоянов: \"Аре в лафката!\"");
        }
        else if (_state == ShopState.Talking3)
        {
            Console.WriteLine("Стоянов: \"Айдее!\"\nВ следващият миг се оказваш в лафката!\nЛавката е отключена! Можеш да купуваш предмети до лимита си на тежест. Предметите могат да се използват от менюто с характеристики или по време на битка.");
        }
        else if (_state == ShopState.Repeated)
        {
            Console.WriteLine("Стоянов: \"Еййй мой човек!\"\n\"Аре лафчос.\"");
        }
        else if (_state == ShopState.Repeated2)
        {
            Console.WriteLine("Стоянов: \"Ееее РАЗвалиха я тая държава!\"");
            if (engine.State.Flags.ContainsKey("relics_unlocked") && !engine.State.Flags.ContainsKey("obtained_deodorant"))
            {
                Console.WriteLine("Стоянов: \"а да вземи това нещо\"");
            }
        }
        else if (_state == ShopState.Repeated3)
        {
            Console.WriteLine("Стоянов стои и яде пица.");
        }

        Console.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
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
