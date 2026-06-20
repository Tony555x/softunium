using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Skills;

namespace Harduni.Panels;

public class TempoSkillListPanel : IPanel
{
    private List<Option> _options = new List<Option>();
    private bool _showAll = false;
    private string _message = "";
    private int _page = 0;
    private const int _pageSize = 8;

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;
        bool inBattle = engine.CurrentPanel == engine.State.World.BattlePanel;

        var skillsToShow = inBattle ? p.EquippedTempoSkills : p.TempoSkills;
        var filteredSkills = new List<Skill>();

        for (int i = 0; i < skillsToShow.Count; i++)
        {
            var skill = skillsToShow[i];
            bool isEquipped = p.EquippedTempoSkills.Contains(skill);
            
            if (!inBattle && !isEquipped && !_showAll) continue;
            
            filteredSkills.Add(skill);
        }

        int totalSkills = filteredSkills.Count;
        int startIndex = 0;
        int endIndex = totalSkills;

        if (!inBattle)
        {
            startIndex = _page * _pageSize;
            endIndex = Math.Min(startIndex + _pageSize, totalSkills);
            
            if (startIndex >= totalSkills && _page > 0)
            {
                _page = Math.Max(0, (totalSkills - 1) / _pageSize);
                startIndex = _page * _pageSize;
                endIndex = Math.Min(startIndex + _pageSize, totalSkills);
            }
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            var skill = filteredSkills[i];
            bool isEquipped = p.EquippedTempoSkills.Contains(skill);
            
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

            string info = skill.GetDetailedDescription();
            if (skill.BaseCooldown > 0) info += $"\nИзчакване: {skill.BaseCooldown} хода.";

            string costText = "";
            if (skill.TempoCost > 0) costText += $"{skill.TempoCost} Темпо";
            if (skill.TempoCost > 0 && skill.MpCost > 0) costText += " и ";
            if (skill.MpCost > 0) costText += $"{skill.MpCost} Айрян";
            if (string.IsNullOrEmpty(costText)) costText = "0 Темпо";

            int displayId = i - startIndex + 1;

            _options.Add(new Option(displayId, $"{prefix}{skill.Name}{cdText}{currentCdText} ({costText}): {skill.ShortDescription}", info, (eng) =>
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
                    // Tempo skills are not usable outside battle, but keep logic generic
                    if (p.Mp < skill.MpCost)
                    {
                        _message = $"Нямате достатъчно Айрян за {skill.Name}!";
                    }
                    else if (p.Tempo < skill.TempoCost)
                    {
                        _message = $"Нямате достатъчно Темпо за {skill.Name}!";
                    }
                    else
                    {
                        p.Mp -= skill.MpCost;
                        p.Tempo -= skill.TempoCost;
                        string msg = skill.Execute(p, new System.Collections.Generic.List<Harduni.Enemies.Enemy>(), null);
                        _message = msg;
                    }
                }
            }, isDisabled, skill.Name));
        }

        if (!inBattle && _page > 0)
        {
            _options.Add(new Option(-1, "<< Предишна страница", "", (eng) => 
            {
                _page--;
                BuildOptions(eng);
            }));
        }

        if (!inBattle && endIndex < totalSkills)
        {
            _options.Add(new Option(-2, "Следваща страница >>", "", (eng) => 
            {
                _page++;
                BuildOptions(eng);
            }));
        }

        if (!inBattle)
        {
            string toggleText = _showAll ? "Скрий неекипирани умения" : "Покажи всички умения";
            _options.Add(new Option(0, toggleText, "Превключва показването на всички притежавани умения.", (eng) => 
            {
                _showAll = !_showAll;
                _page = 0;
                BuildOptions(eng);
            }));
        }
    }

    public void OnOpen(GameEngine engine)
    {
        _page = 0;
        BuildOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== ТЕМПО УМЕНИЯ ===");
        if (!string.IsNullOrEmpty(_message)) VConsole.WriteLine($"\n{_message}");
        VConsole.WriteLine("\nИзберете темпо умение (или натиснете Enter за връщане):");
        
        if (_options.Count == 0)
        {
            VConsole.WriteLine(" Нямате темпо умения.");
            return;
        }

        bool inBattle = engine.CurrentPanel == engine.State.World.BattlePanel;
        if (!inBattle)
        {
            VConsole.WriteLine($"Страница: {_page + 1}");
        }

        foreach (var opt in _options)
        {
            if (opt.Id == -1 || opt.Id == -2 || opt.Id == 0)
            {
                VConsole.WriteLine($" { (opt.Id <= 0 ? (opt.Id == 0 ? "0" : (opt.Id == -1 ? "P" : "N")) : opt.Id.ToString()) }. {opt.Text}");
                continue;
            }
            if (opt.IsDisabled) VConsole.ForegroundColor = ConsoleColor.DarkGray;
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
            VConsole.ResetColor();
        }

        if (!inBattle)
        {
            VConsole.WriteLine("\n[P - Предишна страница | N - Следваща страница]");
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

        string normalizedInput = input.Trim().ToUpper();
        if (!inBattle && normalizedInput == "P")
        {
            var opt = _options.Find(o => o.Id == -1);
            opt?.OnSelect?.Invoke(engine);
            return;
        }
        if (!inBattle && normalizedInput == "N")
        {
            var opt = _options.Find(o => o.Id == -2);
            opt?.OnSelect?.Invoke(engine);
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
