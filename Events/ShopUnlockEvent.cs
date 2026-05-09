using System;
using Harduni.Core;

namespace Harduni.Events;

public class ShopUnlockEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== ТАЕН МАГАЗИН ===");
        Console.WriteLine("Намирате таен търговец, който ви предлага достъп до своите стоки!");
        Console.WriteLine(" 1. Отключи Магазина");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (input == "1")
        {
            engine.State.Flags["shop_unlocked"] = "true";
            
            var data = engine.State.DungeonData;
            data.IsEventActive = false;
        }
    }
}
