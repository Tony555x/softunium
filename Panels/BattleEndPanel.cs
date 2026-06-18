using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Panels;

public class BattleEndPanel : IPanel
{
    private bool _evaluated;
    private List<string> _levelUpMessages;

    public BattleEndPanel()
    {
        _evaluated = false;
        _levelUpMessages = new List<string>();
    }

    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        var data = engine.State.BattleData;
        
        if (!_evaluated)
        {
            engine.State.Player.AddXp(data.XpGained);
            engine.State.Player.Money += data.MoneyGained;
            _levelUpMessages = engine.State.Player.ProcessLevelUps();
            
            engine.State.Player.Status.ClearNonPersistent();
            engine.State.Player.TriggerEvent(GameEvent.CombatEnd, new CombatEndContext());
            engine.State.Player.RecalcStats();
            
            _evaluated = true;
        }

        VConsole.WriteLine("=== БИТКАТА ПРИКЛЮЧИ ===");
        
        VConsole.WriteLine($"\nОпит: +{data.XpGained} (Общо: {engine.State.Player.Xp}/{engine.State.Player.MaxXp})");
        VConsole.WriteLine($"Пари: +{data.MoneyGained} Лв. (Общо: {engine.State.Player.Money})");
        
        foreach (var msg in _levelUpMessages)
        {
            VConsole.WriteLine(msg);
        }
        
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        _evaluated = false;
        engine.State.BattleData.CurrentSubPanel = null;
        
        var source = engine.State.BattleData.SourcePanel;
        if (source != null)
        {
            engine.ChangeRootPanel(source);
        }
        else
        {
            engine.ChangeRootPanel(engine.State.World.WisdomDungeon); // Fallback
        }
    }
}
