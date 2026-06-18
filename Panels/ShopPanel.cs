using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;
using Harduni.Items;

namespace Harduni.Panels;

public class ShopPanel : IPanel
{
    private List<Option> _options = new();
    private string _message = "";
    private bool _isSellMode = false;
    
    private readonly List<Item> _shopItems = new()
    {
        new Banitsa(),
        new Water(),
        new Candy()
    };

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;
        int optId = 1;

        if (!_isSellMode)
        {
            _options.Add(new Option(optId++, "Режим: Продаване", "Превключване към продажба на ваши предмети.", (eng) =>
            {
                _isSellMode = true;
                _message = "";
                BuildOptions(eng);
            }));

            foreach (var item in _shopItems)
            {
                _options.Add(new Option(optId++, $"Купи {item.Name} ({item.Value} Лева)", $"{item.Description} Тегло: {item.Weight}", (eng) =>
                {
                    if (p.Money >= item.Value)
                    {
                        var newItem = ItemFactory.CreateItem(item.Name) ?? (Item)Activator.CreateInstance(item.GetType());
                        if (p.Inventory.AddItem(newItem, p.MaxWeight))
                        {
                            p.Money -= item.Value;
                            _message = $"Купихте {item.Name}.";
                        }
                        else
                        {
                            _message = "Нямате достатъчно място (тегло) в инвентара!";
                        }
                    }
                    else
                    {
                        _message = "Нямате достатъчно пари!";
                    }
                    BuildOptions(eng);
                }, false, item.Name));
            }
        }
        else
        {
            _options.Add(new Option(optId++, "Режим: Купуване", "Превключване към покупка на предмети от магазина.", (eng) =>
            {
                _isSellMode = false;
                _message = "";
                BuildOptions(eng);
            }));

            if (p.Inventory.Count == 0)
            {
                _options.Add(new Option(optId++, "(Нямате предмети за продажба)", "Инвентарът ви е празен.", (eng) => { }, true));
            }
            else
            {
                for (int i = 0; i < p.Inventory.Count; i++)
                {
                    var item = p.Inventory[i];
                    _options.Add(new Option(optId++, $"Продай {item.Name} ({item.Value} Лева) [Бр: {item.Amount}]", item.Description, (eng) =>
                    {
                        p.Money += item.Value;
                        p.Inventory.RemoveItem(item);
                        _message = $"Продадохте {item.Name} за {item.Value} Лева.";
                        BuildOptions(eng);
                    }, false, item.Name));
                }
            }
        }

        _options.Add(new Option(0, "Изход", "Напуснете лафката.", (eng) => 
        {
            eng.ReturnToPreviousRoot();
        }));
    }

    public void OnOpen(GameEngine engine)
    {
        _isSellMode = false;
        BuildOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        var p = engine.State.Player;
        string modeText = _isSellMode ? "ПРОДАВАНЕ" : "КУПУВАНЕ";
        VConsole.WriteLine($"=== ЛАФКА - {modeText} (Пари: {p.Money} Лева | Тегло: {p.Inventory.TotalWeight}/{p.MaxWeight}) ===");
        if (!string.IsNullOrEmpty(_message)) VConsole.WriteLine($"\n{_message}");
        
        VConsole.WriteLine(_isSellMode ? "\nКакво ще продадете?" : "\nКакво ще купите?");
        foreach (var opt in _options)
        {
            string baseValueDisplay = string.IsNullOrEmpty(opt.BaseValue) ? "" : $" [{opt.BaseValue}]";
            VConsole.WriteLine($" {(opt.Id == 0 ? "0" : opt.Id.ToString())}. {opt.Text}{baseValueDisplay}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        _message = "";
        if (string.IsNullOrWhiteSpace(input))
        {
            engine.ReturnToPreviousRoot();
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption, info => _message = info))
        {
            if (!selectedOption.IsDisabled)
            {
                selectedOption.OnSelect?.Invoke(engine);
            }
        }
    }
}
