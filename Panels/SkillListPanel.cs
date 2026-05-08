using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Panels;

public class SkillListPanel : IPanel
{
    private List<Option> _options = new List<Option>();
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;
        bool inBattle = engine.CurrentPanel == engine.State.World.BattlePanel;

        for (int i = 0; i < p.Skills.Count; i++)
        {
            var skill = p.Skills[i];
            bool isDisabled = inBattle ? !skill.UsableInBattle : !skill.UsableOutsideBattle;
            
            _options.Add(new Option(i + 1, $"{skill.Name} ({skill.MpCost} Айрян): {skill.ShortDescription}", skill.AccurateDescription, (eng) =>
            {
                if (inBattle)
                {
                    eng.State.BattleData.SelectedSkill = skill;
                    if (skill.Target == Harduni.Skills.TargetType.Enemy)
                        eng.State.BattleData.CurrentSubPanel = eng.State.World.TargetSelectionPanel;
                    else
                        eng.State.World.TargetSelectionPanel.ExecuteSkill(null, eng);
                }
                else
                {
                    if (p.Mp < skill.MpCost)
                    {
                        _message = $"Нямате достатъчно Айрян за {skill.Name}!";
                    }
                    else
                    {
                        p.Mp -= skill.MpCost;
                        string msg = skill.Execute(p, new System.Collections.Generic.List<Harduni.Enemies.Enemy>(), null);
                        _message = msg;
                    }
                }
            }, isDisabled, skill.Name));
        }
    }

    public void Render(GameEngine engine)
    {
        BuildOptions(engine);
        Console.WriteLine("=== УМЕНИЯ ===");
        if (!string.IsNullOrEmpty(_message)) Console.WriteLine($"\n{_message}");
        Console.WriteLine("\nИзберете умение (или натиснете Enter за връщане):");
        
        if (_options.Count == 0)
        {
            Console.WriteLine(" Нямате умения.");
            return;
        }

        foreach (var opt in _options)
        {
            if (opt.IsDisabled) Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" {opt.Id}. {opt.Text}");
            Console.ResetColor();
        }
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
