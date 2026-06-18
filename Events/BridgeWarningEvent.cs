using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Events;

public class BridgeWarningEvent : IPanel
{
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        _options.Clear();
        _options.Add(new Option(1, "Продължи", "Насочете се към моста.", (eng) => eng.State.DungeonData.IsEventActive = false));
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== МОСТЪТ НАД ОТРОВАТА ===");
        VConsole.WriteLine("Пътеката пред вас е блокирана от езеро от отрова. Докато заобикаляте, намирате че островът ви е напълно заобиколен, освен дълъг мост водещ до другата страна на стаята. Мостът е охраняван от силен противник!");
        
        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
