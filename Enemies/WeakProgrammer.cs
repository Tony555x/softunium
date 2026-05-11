using Harduni.Core;

namespace Harduni.Enemies;

public class WeakProgrammer : Enemy
{
    public WeakProgrammer() : base(
        name: "Слаб Програмист",
        maxHp: 30,
        attack: 6,
        defence: 3,
        speed: 4,
        magic: 6,
        wisdom: 0,
        luck: 1,
        xpReward: 5,
        moneyReward: 5) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = this.PerformAttack(p, this.Attack);
        engine.State.BattleData.Log($"{Name} ви атакува с бавен код и нанесе {ctx.DamageTaken} щети!");
    }
}
