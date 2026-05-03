using System;
using Harduni.Core;

namespace Harduni.Panels;

public class InventoryPanel : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        var p = engine.State.Player;
        Console.WriteLine("\n=== ИНВЕНТАР ===");
        if (p.Inventory.Count == 0)
        {
            Console.WriteLine("Инвентарът ви е празен.");
        }
        else
        {
            for (int i = 0; i < p.Inventory.Count; i++)
            {
                var item = p.Inventory[i];
                Console.WriteLine($" {i + 1}. {item.Name}: {item.Description}");
            }
        }
        Console.WriteLine("\n[Въведете номер за използване, или натиснете Enter за затваряне]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            if (engine.State.BattleData != null && engine.State.BattleData.CurrentSubPanel == this)
            {
                engine.State.BattleData.CurrentSubPanel = null;
            }
            else
            {
                engine.ReturnToPreviousRoot();
            }
            return;
        }

        var p = engine.State.Player;
        if (int.TryParse(input, out int selection))
        {
            if (selection > 0 && selection <= p.Inventory.Count)
            {
                var item = p.Inventory[selection - 1];
                item.OnUse?.Invoke(p);
                p.Inventory.RemoveAt(selection - 1);
                
                if (engine.State.BattleData != null && engine.State.BattleData.CurrentSubPanel == this)
                {
                    engine.State.BattleData.BattleMessage = $"Използвахте {item.Name}.";
                    engine.State.BattleData.CurrentSubPanel = null;
                    engine.State.BattleData.IsPlayerTurn = false;
                }
            }
        }
    }
}
