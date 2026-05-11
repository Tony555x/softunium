using System;
using System.Linq;
using Harduni.Core;
using Harduni.Models;
using Harduni.Skills;

namespace Harduni.Panels;

public class TargetSelectionPanel : IPanel
{
    public TargetSelectionPanel()
    {
    }

    public void Update(float deltaTime, GameEngine engine) { }
    public void OnOpen(GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        var data = engine.State.BattleData;
        Console.WriteLine($"\nИзберете цел за {data.SelectedSkill.Name} (или натиснете Enter за връщане):");
        
        var livingEnemies = data.Enemies.Where(e => e.Hp > 0).ToList();
        for (int i = 0; i < livingEnemies.Count; i++)
        {
            Console.WriteLine($" {i + 1}. {livingEnemies[i].Name}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        var data = engine.State.BattleData;
        if (string.IsNullOrWhiteSpace(input))
        {
            data.CurrentSubPanel = null; 
            return;
        }

        if (int.TryParse(input, out int selection))
        {
            var livingEnemies = data.Enemies.Where(e => e.Hp > 0).ToList();
            if (selection > 0 && selection <= livingEnemies.Count)
            {
                var target = livingEnemies[selection - 1];
                ExecuteSkill(target, engine);
            }
        }
    }

    public void ExecuteSkill(Harduni.Enemies.Enemy target, GameEngine engine)
    {
        var data = engine.State.BattleData;
        var p = engine.State.Player;
        var skill = data.SelectedSkill;

        if (p.Mp < skill.MpCost)
        {
            data.Log($"Нямате достатъчно Айрян за {skill.Name}!");
            data.CurrentSubPanel = null;
            return;
        }

        p.Mp -= skill.MpCost;

        string resultMsg = skill.Execute(p, data.Enemies, target);
        
        data.Log($"Използвахте {skill.Name}. {resultMsg}");
        
        p.TriggerEvent(GameEvent.EndTurn, new TurnContext(engine));
        data.IsPlayerTurn = false; // END PLAYER TURN
        data.CurrentSubPanel = null;
    }
}
