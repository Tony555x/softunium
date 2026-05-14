using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Models;
using Harduni.Skills;

namespace Harduni.Panels;

public class SkillLoadoutPanel : IPanel
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
        _slotStatus = $"Слотове за умения: {p.EquippedSkills.Count}/{p.MaxSkillSlots}";

        int startIndex = _page * _pageSize;
        int endIndex = Math.Min(startIndex + _pageSize, p.Skills.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            var skill = p.Skills[i];
            bool isEquipped = p.EquippedSkills.Contains(skill);
            string status = isEquipped ? "[ЕКИПИРАНО]" : "[НЕЕКИПИРАНО]";
            
            string info = skill.AccurateDescription;
            foreach (var kw in skill.Keywords)
            {
                string explanation = SkillKeywords.GetExplanation(kw);
                if (!string.IsNullOrEmpty(explanation))
                {
                    info += "\n" + explanation;
                }
            }

            _options.Add(new Option(i + 1, $"{status} {skill.Name}: {skill.ShortDescription}", info, (eng) => 
            {
                if (isEquipped)
                {
                    p.EquippedSkills.Remove(skill);
                    _message = $"Премахнахте {skill.Name}.";
                }
                else
                {
                    if (p.EquippedSkills.Count >= p.MaxSkillSlots)
                    {
                        _message = "Нямате свободни слотове!";
                    }
                    else
                    {
                        p.EquippedSkills.Add(skill);
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

        if (endIndex < p.Skills.Count)
        {
            _options.Add(new Option(-2, "Следваща страница >>", "", (eng) => 
            {
                _page++;
                BuildOptions(eng);
            }));
        }

        _options.Add(new Option(0, "Изчисти", "Маха всички екипирани умения.", (eng) => 
        {
            p.EquippedSkills.Clear();
            p.RecalcStats();
            _message = "Всички умения са премахнати.";
            BuildOptions(eng);
        }));
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== ЕКИПИРОВКА НА УМЕНИЯ ===");
        Console.WriteLine(_slotStatus);
        if (!string.IsNullOrEmpty(_message)) 
        {
            Console.WriteLine();
            Console.WriteLine(_message);
        }
        Console.WriteLine($"\nСтраница: {_page + 1}");
        Console.WriteLine("\nИзберете умение за екипиране/премахване:");
        
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
