using Harduni.Core;

namespace Harduni.Enemies;

public class Beta : Enemy
{
    public Beta() : base("Бета", 20, 6, 3, 5, 5, 0, 5, 5) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = this.PerformAttack(p, this.Attack);
        engine.State.BattleData.BattleMessage = $"{Name} ви удари силно и нанесе {ctx.DamageTaken} щети!";
    }
}
