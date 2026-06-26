using System;
using Harduni.Core;

namespace Harduni.Panels;

public class DeathPanel : IPanel
{
    private float _timer = 0f;
    bool once=false;

    public void OnOpen(GameEngine engine)
    {
        _timer = 0f;
        once = false;
    }

    public void Update(float deltaTime, GameEngine engine)
    {
        _timer += deltaTime;
    }

    public void Render(GameEngine engine)
    {
        if (_timer < 2f)
        {
            VConsole.Clear();
            once=true;
            return;
        }
        if(once)
        {
            VConsole.Clear();
            once=false;
        }

        VConsole.ForegroundColor = ConsoleColor.Red;
        VConsole.WriteLine("\n\n");
        VConsole.WriteLine("          ========================================          ");
        VConsole.WriteLine("                       В И Е   У М Р Я Х Т Е                ");
        VConsole.WriteLine("          ========================================          ");
        VConsole.ResetColor();
        VConsole.WriteLine("\n\n          Натиснете Enter, за да се върнете в Кордор...");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (_timer >= 2f)
        {
            // Reset death states just in case
            engine.State.Player.Hp = engine.State.Player.MaxHp;
            engine.State.Player.Mp = engine.State.Player.MaxMp;
            _timer = 0f;
            once = false;
            engine.State.BattleData.PlayerDeathTimer = 0f; // clean up state
            
            engine.State.DungeonData.IsInDungeon = false;
            engine.State.Player.Status.ClearAll();
            engine.State.Player.RecalcStats();
            
            engine.ChangeRootPanel(engine.State.World.Kordor);
        }
    }
}
