using System;
using System.Collections.Generic;
using Harduni.Core;
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
            string limitStr = (item.MaxStacks > 1) ? $" (Макс: {item.MaxStacks})" : "";
            if (item.MaxStacks == -1) limitStr = "";

            _options.Add(new Option(i + 1, $"{item.Name}{amountStr}: {item.Description}{limitStr}", item.AccurateDescription, (eng) =>
            {
                string result = item.Use(p);
                p.Inventory.RemoveItem(item);
                
                if (inBattle)
                {
                    eng.State.BattleData.Log($"{result}");
                    eng.State.BattleData.CurrentSubPanel = null;
                    eng.State.BattleData.IsPlayerTurn = false;
                }
                else
                {
                    _message = result;
                }
            }, isDisabled, item.Name));
        }
    }

    public void Render(GameEngine engine)
    {
        BuildOptions(engine);
        Console.WriteLine("=== ИНВЕНТАР ===");
        if (!string.IsNullOrEmpty(_message)) Console.WriteLine($"\n{_message}");
        
        if (_options.Count == 0)
        {
            Console.WriteLine("Инвентарът ви е празен.");
        }
        else
        {
            foreach (var opt in _options)
            {
                if (opt.IsDisabled) Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" {opt.Id}. {opt.Text}");
                Console.ResetColor();
            }
        }
        Console.WriteLine("\n[Въведете номер за използване, ? номер за инфо, или Enter за затваряне]");
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
            if (inBattle) engine.State.BattleData.Log(info);
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
