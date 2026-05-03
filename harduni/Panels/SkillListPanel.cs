using System;
using Harduni.Core;

namespace Harduni.Panels;

public class SkillListPanel : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        var p = engine.State.Player;
        Console.WriteLine("\nИзберете умение (или натиснете Enter за връщане):");
        
        for (int i = 0; i < p.Skills.Count; i++)
        {
            var skill = p.Skills[i];
            Console.WriteLine($" {i + 1}. {skill.Name} ({skill.MpCost} Айрян)");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            engine.State.BattleData.CurrentSubPanel = null;
            return;
        }

        if (int.TryParse(input, out int selection))
        {
            var p = engine.State.Player;
            if (selection > 0 && selection <= p.Skills.Count)
            {
                engine.State.BattleData.SelectedSkill = p.Skills[selection - 1];
                engine.State.BattleData.CurrentSubPanel = engine.State.World.TargetSelectionPanel;
            }
        }
    }
}
