using System;
using Harduni.Core;

namespace Harduni.Panels;

public class ExampleEvent : IPanel
{
    public void Update(float deltaTime, GameEngine engine) { }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\nИзворче на късмета: Искате ли да пиете от него?");
        Console.WriteLine(" 1. Пий (възстановява 5 Живот)");
        Console.WriteLine(" 2. Измий се (губи 5 Живот)");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        var data = engine.State.DungeonData;
        var p = engine.State.Player;

        if (input == "1")
        {
            p.Hp = Math.Min(p.MaxHp, p.Hp + 5);
            data.IsEventActive = false;
            data.Rooms[data.CurrentRoomIndex].IsCleared = true;
        }
        else if (input == "2")
        {
            p.Hp -= 5;
            if (p.Hp <= 0) p.Hp = 0;
            data.IsEventActive = false;
            data.Rooms[data.CurrentRoomIndex].IsCleared = true;
        }
    }
}
