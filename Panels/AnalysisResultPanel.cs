using System;
using Harduni.Core;
using Harduni.Models;
using Harduni.Skills;

namespace Harduni.Panels;

public class AnalysisResultPanel : IPanel
{
    private Entity _target;

    public void SetTarget(Entity target)
    {
        _target = target;
    }

    public void Update(float deltaTime, GameEngine engine) { }
    public void OnOpen(GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        if (_target == null) return;
        
        VConsole.WriteLine($"\n=== АНАЛИЗ: {_target.Name} ===");
        VConsole.WriteLine($"Живот: {_target.Hp}/{_target.MaxHp} | Айрян: {_target.Mp}/{_target.MaxMp}");
        VConsole.WriteLine($"Атака: {_target.Attack} | Защита: {_target.Defence}");
        VConsole.WriteLine($"Скорост: {_target.Speed} | Магия: {_target.Magic}");
        VConsole.WriteLine($"Мъдрост: {_target.Wisdom} | Късмет: {_target.Luck}");
        
        VConsole.WriteLine("\nЕфекти:");
        var statuses = _target.Status.Statuses;
        if (statuses.Count == 0)
        {
            VConsole.WriteLine(" Няма активни ефекти.");
        }
        else
        {
            foreach (var status in statuses)
            {
                string line = $" - {status.GetDisplayString()}: {status.GetDescription()}";
                foreach (var kw in status.Keywords)
                {
                    string explanation = Skill.GetKeywordExplanation(kw);
                    if (!string.IsNullOrEmpty(explanation))
                    {
                        line += "\n   > " + explanation;
                    }
                }
                VConsole.WriteLine(line);
            }
        }
        VConsole.WriteLine("\n[Натиснете Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = null;
    }
}
