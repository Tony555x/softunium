using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;
using Harduni.Items;

namespace Harduni.Panels;

public class InventoryPanel : IPanel
{
    private List<Option> _options = new List<Option>();
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;
        bool inBattle = engine.CurrentPanel == engine.State.World.BattlePanel;

        for (int i = 0; i < p.Inventory.Count; i++)
        {
            var item = p.Inventory[i];
            bool isDisabled = inBattle ? !item.UsableInBattle : !item.UsableOutsideBattle;
            
            string amountStr = item.Amount > 1 ? $" [x{item.Amount}]" : "";
            string weightStr = $" (Тегло: {item.Weight * item.Amount})";

            _options.Add(new Option(i + 1, $"{item.Name}{amountStr}: {item.Description}{weightStr}", item.AccurateDescription, (eng) =>
            {
                string result = item.Use(p);
                p.Inventory.RemoveItem(item);
                
                if (inBattle)
                {
                    eng.State.BattleData.Log($"{result}");
                    eng.State.BattleData.CurrentSubPanel = null;
                    p.TriggerEvent(GameEvent.EndTurn, new TurnContext(eng));
                    eng.State.BattleData.IsPlayerTurn = false;
                }
                else
                {
                    _message = result;
                }
            }, isDisabled, item.Name));
        }
    }

    public void OnOpen(GameEngine engine)
    {
        BuildOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        var p = engine.State.Player;
        VConsole.WriteLine($"=== ИНВЕНТАР (Тегло: {p.Inventory.TotalWeight}/{p.MaxWeight}) ===");
        if (!string.IsNullOrEmpty(_message)) VConsole.WriteLine($"\n{_message}");
        
        if (_options.Count == 0)
        {
            VConsole.WriteLine("Инвентарът ви е празен.");
        }
        else
        {
            foreach (var opt in _options)
            {
                if (opt.IsDisabled) VConsole.ForegroundColor = ConsoleColor.DarkGray;
                VConsole.WriteLine($" {opt.Id}. {opt.Text}");
                VConsole.ResetColor();
            }
        }
        VConsole.WriteLine("\n[Въведете номер за използване, ? номер за инфо, или Enter за затваряне]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        _message = "";
        bool inBattle = engine.CurrentPanel == engine.State.World.BattlePanel;
        if (string.IsNullOrWhiteSpace(input))
        {
            if (inBattle) engine.State.BattleData.CurrentSubPanel = null;
            else ((StatsPanel)engine.State.World.StatsPanel).CurrentSubPanel = null;
            return;
        }

        BuildOptions(engine);

        if (!InputHandler.Handle(input, _options, out Option selectedOption, info => 
        {
            if (inBattle)
            {
                engine.State.BattleData.ClearLog();
                engine.State.BattleData.Log(info);
            }
            else _message = info;
        }))
        {
            if (selectedOption.IsDisabled)
            {
                if (inBattle) engine.State.BattleData.Log("Не можете да използвате това в момента.");
                else _message = "Не можете да използвате това в момента.";
            }
            else
            {
                selectedOption.OnSelect?.Invoke(engine);
            }
        }
    }
}
