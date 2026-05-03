using Harduni.Core;

namespace Harduni.Enemies;

public class WeakProgrammer : Enemy
{
    public WeakProgrammer() : base("Слаб Програмист", 30, 5, 2, 4, 5, 0, 5, 5) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = this.PerformAttack(p, this.Attack);
        engine.State.BattleData.BattleMessage = $"{Name} ви атакува с бавен код и нанесе {ctx.DamageTaken} щети!";
    }
}
