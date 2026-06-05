using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Panels;

public class StatsPanel : IPanel
{
    public IPanel? CurrentSubPanel { get; set; }
    private List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        bool inBattle = engine.PreviousRootPanel == engine.State.World.BattlePanel;
        bool isShopUnlocked = engine.State.Flags.ContainsKey("shop_unlocked");

        int id = 1;
        if (isShopUnlocked)
        {
            _options.Add(new Option(id++, "Инвентар", "Вижте своите предмети.", (eng) => { CurrentSubPanel = eng.State.World.InventoryPanel; CurrentSubPanel.OnOpen(eng); }));
        }
        
        _options.Add(new Option(id++, "Умения", "Вижте своите умения.", (eng) => { CurrentSubPanel = eng.State.World.SkillListPanel; CurrentSubPanel.OnOpen(eng); }));

        if (!inBattle)
        {
            _options.Add(new Option(id++, "Запис", "Запишете играта в някой от слотовете.", (eng) => { CurrentSubPanel = new SaveSlotPanel(true); CurrentSubPanel.OnOpen(eng); }));
            _options.Add(new Option(id++, "Зареждане", "Заредете играта от някой от слотовете.", (eng) => { CurrentSubPanel = new SaveSlotPanel(false); CurrentSubPanel.OnOpen(eng); }));
        }
    }

    public void OnOpen(GameEngine engine)
    {
        BuildOptions(engine);
        CurrentSubPanel = null;
    }

    public void Render(GameEngine engine)
    {
        if (CurrentSubPanel != null)
        {
            var player = engine.State.Player;
            if(CurrentSubPanel is not SaveSlotPanel)
            {
                DrawBar("Живот ", player.Hp, player.MaxHp);
                DrawBar("Айрян ", player.Mp, player.MaxMp);
                Console.WriteLine();
            }
            CurrentSubPanel.Render(engine);
            return;
        }

        var p = engine.State.Player;
        Console.WriteLine("=== ХАРАКТЕРИСТИКИ И УМЕНИЯ ===");
        Console.WriteLine($"Име: {p.Name} ({p.BattleName})");
        Console.WriteLine($"Ниво: {p.Level}");
        Console.WriteLine($"Пари: {p.Money} Лева");
        
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
        Console.WriteLine($"Магия    : {p.Magic}");
        Console.WriteLine($"Мъдрост  : {p.Wisdom}");
        Console.WriteLine($"Късмет   : {p.Luck}");

        Console.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            Console.WriteLine($" [{opt.Id}] {opt.Text}");
        }
        Console.WriteLine("\n[Enter - Затваряне]");
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
        if (CurrentSubPanel != null)
        {
            CurrentSubPanel.ProcessInput(input, engine);
            return;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            engine.ReturnToPreviousRoot();
            return;
        }

        BuildOptions(engine);
        
        // Handle И/У shortcuts first for compatibility
        if (input.Equals("и", StringComparison.OrdinalIgnoreCase) || input.Equals("I", StringComparison.OrdinalIgnoreCase))
        {
            if (engine.State.Flags.ContainsKey("shop_unlocked"))
            {
                CurrentSubPanel = engine.State.World.InventoryPanel;
                CurrentSubPanel.OnOpen(engine);
            }
            return;
        }
        if (input.Equals("у", StringComparison.OrdinalIgnoreCase) || input.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            CurrentSubPanel = engine.State.World.SkillListPanel;
            CurrentSubPanel.OnOpen(engine);
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
