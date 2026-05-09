using System;
using System.Collections.Generic;
using System.Linq;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Panels;

public class BattlePanel : IPanel
{
    private List<Option> _options;

    public BattlePanel()
    {
        _options = new List<Option>();
        _options.Add(new Option(1, "Атака", "Извършва основната атака.", OpenAttackTargeting));
        _options.Add(new Option(2, "Умения", "Списък с вашите умения.", OpenSkills));
        _options.Add(new Option(3, "Инвентар", "Отваря инвентара с предмети.", OpenInventory));
        _options.Add(new Option(4, "Бягство", "Бягство от битката.", Escape));
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
        try { width = Console.WindowWidth - 1; } catch { }

        Console.WriteLine("=== БИТКА ===".PadRight(width));
        
        var p = engine.State.Player;
        string pStatusStr = p.Status.GetCombinedDisplayString();
        string playerStats = $"{p.BattleName} | Живот: {p.Hp}/{p.MaxHp} | Айрян: {p.Mp}/{p.MaxMp} | Енергия: {GetEnergyBar(p.Energy, p.EnergyBarSize)} {pStatusStr}";
        
        if (p.Hp <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(playerStats.PadRight(System.Math.Max(playerStats.Length, width)));
        }
        
        Console.WriteLine();
        
        
        // Render last 4 log messages
        for (int i = 0; i < 4; i++)
        {
            int logIndex = data.BattleLog.Count - 4 + i;
            string logLine = logIndex >= 0 ? data.BattleLog[logIndex] : "-";
            Console.WriteLine(logLine.PadRight(width));
        }
        
        Console.WriteLine("\nВрагове:");
        for (int i = 0; i < data.Enemies.Count; i++)
        {
            var e = data.Enemies[i];
            string eStatusStr = e.Status.GetCombinedDisplayString();
            string status = e.Hp > 0 ? $"{e.Hp}/{e.MaxHp} HP | Енергия: {GetEnergyBar(e.Energy, e.EnergyBarSize)} {eStatusStr}" : "МЪРТЪВ".PadRight(25);
            string enemyLine = $"{i + 1}. {e.Name.PadRight(20)} - {status}";
            Console.WriteLine(enemyLine.PadRight(System.Math.Max(enemyLine.Length, width)));
        }

        if (data.CurrentSubPanel != null)
        {
            Console.WriteLine();
            data.CurrentSubPanel.Render(engine);
        }
        else if (data.IsPlayerTurn)
        {
            Console.WriteLine("\nВъзможни действия:");
            foreach (var option in _options)
            {
                Console.WriteLine($" {option.Id}. {option.Text}");
            }
        }
        else
        {
            Console.WriteLine("\nИзчаква се ред...");
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

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }

    private void OpenAttackTargeting(GameEngine engine)
    {
        engine.State.BattleData.SelectedSkill = new Harduni.Skills.BasicAttack();
        engine.State.BattleData.CurrentSubPanel = engine.State.World.TargetSelectionPanel;
    }

    private void OpenSkills(GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = engine.State.World.SkillListPanel;
    }

    private void OpenInventory(GameEngine engine)
    {
        engine.State.BattleData.CurrentSubPanel = engine.State.World.InventoryPanel;
    }

    private void Escape(GameEngine engine)
    {
        engine.State.BattleData.IsPlayerTurn = false;
        engine.ChangeRootPanel(engine.State.World.WisdomRoom);
    }

    private string GetEnergyBar(float energy, int segments)
    {
        int filled = (int)System.Math.Clamp(System.Math.Round((energy / 1000f) * segments), 0, segments);
        return "[" + new string('█', filled) + new string('-', segments - filled) + "]";
    }
}
