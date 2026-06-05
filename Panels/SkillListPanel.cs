using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Skills;

namespace Harduni.Panels;

public class SkillListPanel : IPanel
{
    private List<Option> _options = new List<Option>();
    private bool _showAll = false;
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;
        bool inBattle = engine.CurrentPanel == engine.State.World.BattlePanel;

        var skillsToShow = inBattle ? p.EquippedSkills : p.Skills;

        for (int i = 0; i < skillsToShow.Count; i++)
        {
            var skill = skillsToShow[i];
            bool isEquipped = p.EquippedSkills.Contains(skill);
            
            // In battle, we only show equipped skills anyway.
            // Out of battle, we show all, but if not equipped and _showAll is false, we skip?
            // "add a toggle to show unequipped skills out of battle"
            
            if (!inBattle && !isEquipped && !_showAll) continue;
            
            bool isDisabled = !skill.CanPlay(inBattle);
            if (!isEquipped) isDisabled = true;
            
            string prefix = isEquipped ? "" : "[НЕЕКИПИРАНО] ";
            string cdText = "";
            string currentCdText = "";
            if (inBattle)
            {
                if (skill.BaseCooldown > 0)
                {
                    int charged = Math.Max(0, skill.BaseCooldown - skill.Cooldown);
                    cdText = $" [(~) {charged}/{skill.BaseCooldown}]";
                }
            }
            else
            {
                cdText = skill.BaseCooldown > 0 ? $" [(~) {skill.BaseCooldown}]" : "";
            }

            string info = skill.AccurateDescription;
            if (skill.BaseCooldown > 0) info += $"\nИзчакване: {skill.BaseCooldown} хода.";

            foreach (var kw in skill.Keywords)
            {
                string explanation = SkillKeywords.GetExplanation(kw);
                if (!string.IsNullOrEmpty(explanation))
                {
                    info += "\n" + explanation;
                }
            }

            _options.Add(new Option(_options.Count + 1, $"{prefix}{skill.Name}{cdText}{currentCdText} ({skill.MpCost} Айрян): {skill.ShortDescription}", info, (eng) =>
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

        if (!inBattle)
        {
            string toggleText = _showAll ? "Скрий неекипирани умения" : "Покажи всички умения";
            _options.Add(new Option(0, toggleText, "Превключва показването на всички притежавани умения.", (eng) => 
            {
                _showAll = !_showAll;
                BuildOptions(eng);
            }));
        }
    }

    public void OnOpen(GameEngine engine)
    {
        BuildOptions(engine);
    }

    public void Render(GameEngine engine)
    {
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
