using System;
using Harduni.Core;

namespace Harduni.Panels;

public class StatsPanel : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        var p = engine.State.Player;
        Console.WriteLine("=== ХАРАКТЕРИСТИКИ И УМЕНИЯ ===");
        Console.WriteLine($"Име: {p.Name} ({p.BattleName})");
        Console.WriteLine($"Ниво: {p.Level}");
        
        DrawBar("Живот ", p.Hp, p.MaxHp);
        DrawBar("Айрян ", p.Mp, p.MaxMp);
        DrawBar("Опит  ", p.Xp, p.MaxXp);
        
        string alignmentTitle = p.Alignment == 0 ? "Неутрален (0)" : 
                                p.Alignment > 0 ? $"Петуриум ({p.Alignment})" : 
                                $"Гамениум ({p.Alignment})";
        Console.WriteLine($"\nСклонност: {alignmentTitle}");

        Console.WriteLine("\n--- Атрибути ---");
        Console.WriteLine($"Атака    : {p.Attack}");
        Console.WriteLine($"Защита   : {p.Defence}");
        Console.WriteLine($"Скорост  : {p.Speed}");
        Console.WriteLine($"Точност  : {p.Magic}");
        Console.WriteLine($"Избягване: {p.Wisdom}");
        Console.WriteLine($"Късмет   : {p.Luck}");

        Console.WriteLine("\n=== УМЕНИЯ ===");
        if (p.Skills.Count == 0)
        {
            Console.WriteLine("Нямате умения.");
        }
        else
        {
            foreach (var skill in p.Skills)
            {
                Console.WriteLine($"- {skill.Name} ({skill.MpCost} Айрян): {skill.Description}");
            }
        }

        Console.WriteLine("\n[Натиснете Enter за затваряне]");
    }

    private void DrawBar(string label, int current, int max)
    {
        int barLength = 20;
        int filled = max > 0 ? (int)Math.Round((double)current / max * barLength) : 0;
        filled = Math.Clamp(filled, 0, barLength);
        
        string bar = new string('█', filled) + new string('-', barLength - filled);
        Console.WriteLine($"{label}: [{bar}] {current}/{max}");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.ReturnToPreviousRoot();
    }
}
