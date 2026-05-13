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
        
        Console.WriteLine($"\n=== АНАЛИЗ: {_target.Name} ===");
        Console.WriteLine($"Живот: {_target.Hp}/{_target.MaxHp} | Айрян: {_target.Mp}/{_target.MaxMp}");
        Console.WriteLine($"Атака: {_target.Attack} | Защита: {_target.Defence}");
        Console.WriteLine($"Скорост: {_target.Speed} | Магия: {_target.Magic}");
        Console.WriteLine($"Мъдрост: {_target.Wisdom} | Късмет: {_target.Luck}");
        
        Console.WriteLine("\nЕфекти:");
        var statuses = _target.Status.Statuses;
        if (statuses.Count == 0)
        {
            Console.WriteLine(" Няма активни ефекти.");
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
                Console.WriteLine(line);
            }
        }
        Console.WriteLine("\n[Натиснете Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = null;
    }
}
