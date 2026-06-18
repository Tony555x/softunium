using System;
using Harduni.Core;

namespace Harduni.Events;

public class TeamworkShopUnlockEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        engine.State.Flags["shop_unlocked"] = "true";
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("=== ИЗГУБЕН ТЪРГОВЕЦ ===");
        VConsole.WriteLine("Намирате търговец, който се е изгубил в това фитнес подземие.");
        VConsole.WriteLine("'О, благодаря! Вече ще ме намерите в Кордор, ако имате нужда от стока!'");
        VConsole.WriteLine("\n(Магазинът в Кордор е отключен!)");
        VConsole.WriteLine("\n[Натиснете Enter за продължаване]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        engine.State.DungeonData.IsEventActive = false;
    }
}
