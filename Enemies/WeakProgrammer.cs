using Harduni.Core;
using Harduni.Models;


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
        moneyReward: 1) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
        p.TakeDamage(ctx);
        engine.State.BattleData.Log($"{Name} ви атакува с бавен код и нанесе {ctx.DamageTaken} щети!");
    }
}
