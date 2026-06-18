using System;
using System.Linq;
using Harduni.Core;

namespace Harduni.Panels;

public class AnalysisTargetSelectionPanel : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }
    public void OnOpen(GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        var data = engine.State.BattleData;
        VConsole.WriteLine($"\nИзберете цел за Анализ (0 за вас, 1-N за враг) или натиснете Enter за връщане:");
        
        var livingEnemies = data.Enemies.Where(e => e.Hp > 0).ToList();
        for (int i = 0; i < livingEnemies.Count; i++)
        {
            VConsole.WriteLine($" {i + 1}. {livingEnemies[i].Name}");
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
            if (selection == 0)
            {
                engine.State.World.AnalysisResultPanel.SetTarget(engine.State.Player);
                data.CurrentSubPanel = engine.State.World.AnalysisResultPanel;
                data.CurrentSubPanel.OnOpen(engine);
            }
            else
            {
                var livingEnemies = data.Enemies.Where(e => e.Hp > 0).ToList();
                if (selection > 0 && selection <= livingEnemies.Count)
                {
                    engine.State.World.AnalysisResultPanel.SetTarget(livingEnemies[selection - 1]);
                    data.CurrentSubPanel = engine.State.World.AnalysisResultPanel;
                    data.CurrentSubPanel.OnOpen(engine);
                }
            }
        }
    }
}
