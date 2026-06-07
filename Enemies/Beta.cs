using Harduni.Core;

namespace Harduni.Enemies;

public class Beta : Enemy
{
    public Beta() : base(
        name: "Бета",
        maxHp: 20,
        attack: 8,
        defence: 6,
        speed: 5,
        magic: 8,
        wisdom: 0,
        luck: 1,
        xpReward: 5,
        moneyReward: 2) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
        p.TakeDamage(ctx);
        engine.State.BattleData.Log($"{Name} ви удари силно и нанесе {ctx.DamageTaken} щети!");
    }
}
