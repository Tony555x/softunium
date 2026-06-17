using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Models;
using Harduni.Skills;

namespace Harduni.Panels;

public class TempoSkillLoadoutPanel : IPanel
{
    private List<Option> _options = new();
    private string _message = "";
    private string _slotStatus = "";
    private int _page = 0;
    private const int _pageSize = 20;

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        BuildOptions(engine);
    }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;

        // Slot status
        _slotStatus = $"Слотове за темпо умения: {p.EquippedTempoSkills.Count}/{p.MaxTempoSkillSlots}";

        int startIndex = _page * _pageSize;
        int endIndex = Math.Min(startIndex + _pageSize, p.TempoSkills.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            var skill = p.TempoSkills[i];
            bool isEquipped = p.EquippedTempoSkills.Contains(skill);
            string status = isEquipped ? "[ЕКИПИРАНО]" : "[НЕЕКИПИРАНО]";
            
            string info = skill.GetDetailedDescription();

            string cdStr = skill.BaseCooldown > 0 ? $" [(~) {skill.BaseCooldown}]" : "";
            
            string costText = "";
            if (skill.TempoCost > 0) costText += $"{skill.TempoCost} Темпо";
            if (skill.TempoCost > 0 && skill.MpCost > 0) costText += " и ";
            if (skill.MpCost > 0) costText += $"{skill.MpCost} Айрян";
            if (string.IsNullOrEmpty(costText)) costText = "0 Темпо";

            _options.Add(new Option(i + 1, $"{status} {skill.Name}{cdStr} ({costText}): {skill.ShortDescription}", info, (eng) => 
            {
                if (isEquipped)
                {
                    p.EquippedTempoSkills.Remove(skill);
                    _message = $"Премахнахте {skill.Name}.";
                }
                else
                {
                    if (p.EquippedTempoSkills.Count >= p.MaxTempoSkillSlots)
                    {
                        _message = "Нямате свободни слотове!";
                    }
                    else
                    {
                        p.EquippedTempoSkills.Add(skill);
                        _message = $"Екипирахте {skill.Name}.";
                    }
                }
                p.RecalcStats();
                BuildOptions(eng);
            }, false, skill.Name));
        }

        if (_page > 0)
        {
            _options.Add(new Option(-1, "<< Предишна страница", "", (eng) => 
            {
                _page--;
                BuildOptions(eng);
            }));
        }

        if (endIndex < p.TempoSkills.Count)
        {
            _options.Add(new Option(-2, "Следваща страница >>", "", (eng) => 
            {
                _page++;
                BuildOptions(eng);
            }));
        }

        _options.Add(new Option(0, "Изчисти", "Маха всички екипирани темпо умения.", (eng) => 
        {
            p.EquippedTempoSkills.Clear();
            p.RecalcStats();
            _message = "Всички темпо умения са премахнати.";
            BuildOptions(eng);
        }));
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== ЕКИПИРОВКА НА ТЕМПО УМЕНИЯ ===");
        Console.WriteLine(_slotStatus);
        if (!string.IsNullOrEmpty(_message)) 
        {
            Console.WriteLine();
            Console.WriteLine(_message);
        }
        Console.WriteLine($"\nСтраница: {_page + 1}");
        Console.WriteLine("\nИзберете темпо умение за екипиране/премахване:");
        
        foreach (var opt in _options)
        {
            if (opt.Id == -1 || opt.Id == -2 || opt.Id == 0)
            {
                Console.WriteLine($" { (opt.Id <= 0 ? (opt.Id == 0 ? "0" : (opt.Id == -1 ? "P" : "N")) : opt.Id.ToString()) }. {opt.Text}");
                continue;
            }

            if (opt.Text.StartsWith("[ЕКИПИРАНО]"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" {opt.Id}. [ЕКИПИРАНО]");
                Console.ResetColor();
                Console.WriteLine($" {opt.Text.Substring(11)}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($" {opt.Id}. [НЕЕКИПИРАНО]");
                Console.ResetColor();
                Console.WriteLine($" {opt.Text.Substring(14)}");
            }
        }
        Console.WriteLine("\n[P - Предишна | N - Следваща | 0 - Изчисти]");
        Console.WriteLine("[Натиснете Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            engine.State.World.Kordor.SetSubPanel(engine.State.World.RestPanel);
            engine.State.World.RestPanel.OnOpen(engine);
            return;
        }

        string normalizedInput = input.Trim().ToUpper();
        if (normalizedInput == "P")
        {
            var opt = _options.Find(o => o.Id == -1);
            opt?.OnSelect?.Invoke(engine);
            return;
        }
        if (normalizedInput == "N")
        {
            var opt = _options.Find(o => o.Id == -2);
            opt?.OnSelect?.Invoke(engine);
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption, info => _message = info))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
