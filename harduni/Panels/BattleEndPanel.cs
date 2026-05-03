using System;
using System.Collections.Generic;
using Harduni.Core;

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
            _levelUpMessages = engine.State.Player.ProcessLevelUps();
            _evaluated = true;
        }

        Console.WriteLine("\n=== БИТКАТА ПРИКЛЮЧИ ===");
        Console.WriteLine("Победихте всички врагове!");
        Console.WriteLine($"\nПолучихте {data.XpGained} Опит. ({engine.State.Player.Xp} / {engine.State.Player.MaxXp})");
        
        foreach (var msg in _levelUpMessages)
        {
            Console.WriteLine(msg);
        }
        
        Console.WriteLine("\n[Натиснете Enter за продължаване]");
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
