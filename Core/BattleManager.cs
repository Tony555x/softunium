using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Panels;

namespace Harduni.Core;

public static class BattleManager
{
    public static void StartBattle(GameEngine engine, List<Enemy> enemies, IPanel sourcePanel, float lootMultiplier = 1.0f)
    {
        var data = engine.State.BattleData;
        
        // Initialize Battle Data
        data.Enemies = enemies;
        data.IsFinished = false;
        data.IsPlayerTurn = false;
        data.BattleLog.Clear();
        data.Log("Битката започва!");
        data.CurrentSubPanel = null;
        data.SelectedSkill = null;
        data.XpGained = 0;
        data.MoneyGained = 0;
        data.LootMultiplier = lootMultiplier;
        data.SourcePanel = sourcePanel;

        // Reset Energy and initialize bar sizes
        var p = engine.State.Player;
        p.Energy = 0;
        p.InitialEnergyBarSize = 10;
        p.EnergyBarSize = 10;
        foreach (var skill in p.Skills)
        {
            skill.Cooldown = skill.BaseCooldown + 1;
        }




        foreach (var e in enemies)
        {
            e.Energy = 0;
            e.InitialEnergyBarSize = (int)System.Math.Max(1, 10.0 * p.Speed / e.Speed);//slower enemies should have a longer bar
            e.EnergyBarSize = e.InitialEnergyBarSize;
        }

        engine.ChangeRootPanel(engine.State.World.BattlePanel);
    }
}
