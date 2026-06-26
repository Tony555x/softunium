using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Models;
using Harduni.Skills;
using Harduni.Locations;

namespace Harduni.Panels;

public class BattlePanel : IPanel
{
    private List<Option> _options;

    public BattlePanel()
    {
        _options = new List<Option>();
    }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        int id = 1;
        _options.Add(new Option(id++, "Атака", "Извършва основната атака.", OpenAttackTargeting));
        _options.Add(new Option(id++, "Умения", "Списък с вашите умения.", OpenSkills));
        if (engine.State.Flags.ContainsKey("tempo_unlocked"))
        {
            _options.Add(new Option(id++, "Темпо умения", "Списък с вашите темпо умения.", OpenTempoSkills));
        }
        if (engine.State.Flags.ContainsKey("shop_unlocked"))
        {
            _options.Add(new Option(id++, "Инвентар", "Отваря инвентара с предмети.", OpenInventory));
        }
        _options.Add(new Option(id++, "Анализ", "Прегледайте статистиките и ефектите на целта.", OpenAnalysis));
        _options.Add(new Option(id++, "Бягство", "Бягство от битката.", Escape));
    }

    public void Update(float deltaTime, GameEngine engine)
    {
        var data = engine.State.BattleData;
        if (data.IsFinished) return;

        if (data.CurrentSubPanel != null)
        {
            data.CurrentSubPanel.Update(deltaTime, engine);
        }

        if (data.IsPlayerTurn) return;

        var p = engine.State.Player;
        
        if (p.Hp <= 0)
        {
            if (data.PlayerDeathTimer == 0f)
            {
                data.Log("БЯХТЕ ПОБЕДЕНИ!");
            }

            data.PlayerDeathTimer += deltaTime;

            if (data.PlayerDeathTimer >= 2f)
            {
                data.IsFinished = true;
                data.PlayerDeathTimer = 0f;
                engine.ChangeRootPanel(engine.State.World.DeathPanel);
            }
            return;
        }

        float tickRate = 60f; 

        if (p.Hp > 0)
        {
            p.Energy += p.Speed * deltaTime * tickRate;
            if (p.Energy >= 1000)
            {
                p.Energy -= 1000;
                data.IsPlayerTurn = true;
                data.Log("Ваш ред е!");
                p.TriggerEvent(GameEvent.StartTurn, new TurnContext(engine));
                foreach (var skill in p.Skills)
                {
                    if (skill.Cooldown > 0) skill.Cooldown--;
                }
                foreach (var skill in p.TempoSkills)
                {
                    if (skill.Cooldown > 0) skill.Cooldown--;
                }

                p.RecalcStats();

                return; 
            }
        }

        foreach (var e in data.Enemies)
        {
            if (e.Hp > 0)
            {
                e.Energy += e.Speed * deltaTime * tickRate;
                if (e.Energy >= 1000)
                {
                    e.Energy -= 1000;
                    e.TriggerEvent(GameEvent.StartTurn, new TurnContext(engine));
                    e.RecalcStats();
                    if (e.Hp > 0) e.TakeAction(engine); 
                    e.TriggerEvent(GameEvent.EndTurn, new TurnContext(engine));
                }
            }
        }
        
        if (data.Enemies.TrueForAll(e => e.Hp <= 0))
        {
            data.IsFinished = true;
            data.XpGained = (int)(data.Enemies.Sum(e => e.XpReward) * data.LootMultiplier);
            data.MoneyGained = (int)(data.Enemies.Sum(e => e.MoneyReward) * data.LootMultiplier);
            data.CurrentSubPanel = engine.State.World.BattleEndPanel;
        }
    }

    public void Render(GameEngine engine)
    {
        var data = engine.State.BattleData;
        int width = 80;
        try { width = VConsole.WindowWidth - 1; } catch { }

        VConsole.WriteLine("=== БИТКА ===".PadRight(width));
        
        var p = engine.State.Player;
        string pStatusStr = p.Status.GetCombinedDisplayString();
        string tempoStr = engine.State.Flags.ContainsKey("tempo_unlocked") ? $" | Темпо: {p.Tempo}/{p.MaxTempo}" : "";
        string playerStats = $"{p.BattleName} | Живот: {p.Hp}/{p.MaxHp} | Айрян: {p.Mp}/{p.MaxMp}{tempoStr} | Енергия: {GetEnergyBar(p.Energy, p.EnergyBarSize)} {pStatusStr}";
        
        if (p.Hp <= 0)
        {
            VConsole.ForegroundColor = ConsoleColor.Red;
            VConsole.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
            VConsole.ResetColor();
        }
        else
        {
            VConsole.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
        }
        
        VConsole.WriteLine();
        
        
        // Render last 4 log messages
        for (int i = 0; i < 4; i++)
        {
            int logIndex = data.BattleLog.Count - 4 + i;
            string logLine = logIndex >= 0 ? data.BattleLog[logIndex] : "-";
            VConsole.WriteLine(logLine.PadRight(width));
        }
        
        VConsole.WriteLine("\nВрагове:");
        for (int i = 0; i < data.Enemies.Count; i++)
        {
            var e = data.Enemies[i];
            string eStatusStr = e.Status.GetCombinedDisplayString();
            string status = e.Hp > 0 ? $"{e.Hp}/{e.MaxHp} HP | Енергия: {GetEnergyBar(e.Energy, e.EnergyBarSize)} {eStatusStr}" : "МЪРТЪВ".PadRight(25);
            string enemyLine = $"{i + 1}. {e.Name.PadRight(20)} - {status}";
            VConsole.WriteLine(enemyLine.PadRight(System.Math.Max(enemyLine.Length, width)));
        }

        if (data.CurrentSubPanel != null)
        {
            VConsole.WriteLine();
            data.CurrentSubPanel.Render(engine);
        }
        else if (data.IsPlayerTurn)
        {
            BuildOptions(engine);
            VConsole.WriteLine("\nВъзможни действия:");
            foreach (var option in _options)
            {
                VConsole.WriteLine($" {option.Id}. {option.Text}");
            }
        }
        else
        {
            VConsole.WriteLine("\nИзчаква се ред...");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        var data = engine.State.BattleData;
        
        if (!data.IsPlayerTurn && !data.IsFinished) return;

        if (data.CurrentSubPanel != null)
        {
            data.CurrentSubPanel.ProcessInput(input, engine);
            return;
        }

        BuildOptions(engine);
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }

    private void OpenAttackTargeting(GameEngine engine)
    {
        var skill = new BasicAttack();

        engine.State.BattleData.SelectedSkill = skill;
        engine.State.BattleData.CurrentSubPanel = engine.State.World.TargetSelectionPanel;
        engine.State.World.TargetSelectionPanel.OnOpen(engine);
    }

    private void OpenSkills(GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = engine.State.World.SkillListPanel;
        engine.State.World.SkillListPanel.OnOpen(engine);
    }

    private void OpenTempoSkills(GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = engine.State.World.TempoSkillListPanel;
        engine.State.World.TempoSkillListPanel.OnOpen(engine);
    }

    private void OpenInventory(GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = engine.State.World.InventoryPanel;
        engine.State.World.InventoryPanel.OnOpen(engine);
    }

    private void OpenAnalysis(GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = engine.State.World.AnalysisTargetSelectionPanel;
        engine.State.World.AnalysisTargetSelectionPanel.OnOpen(engine);
    }

    private void Escape(GameEngine engine)
    {
        engine.State.BattleData.IsPlayerTurn = false;
        var source = engine.State.BattleData.SourcePanel;
        
        if (source is Dungeon dungeon)
        {
            // Clear statuses when escaping a dungeon battle, matching dungeon escape logic
            engine.State.Player.Status.ClearAll();
            engine.State.Player.RecalcStats();
            engine.ChangeRootPanel(dungeon.RetreatPanel);
        }
        else
        {
            // Fallback for non-dungeon battles
            engine.ChangeRootPanel(engine.State.World.WisdomRoom);
        }
    }

    private string GetEnergyBar(float energy, int segments)
    {
        int filled = (int)System.Math.Clamp(System.Math.Round((energy / 1000f) * segments), 0, segments);
        return "[" + new string('█', filled) + new string('-', segments - filled) + "]";
    }
}
