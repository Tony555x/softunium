using Harduni.Core;
using Harduni.Models;


namespace Harduni.Enemies;

public class Mangal : Enemy
{
    public Mangal() : base(
        name: "Мангал",
        maxHp: 78,
        attack: 18,
        defence: 14,
        speed: 7,
        magic: 12,
        wisdom: 0,
        luck: 1,
        xpReward: 18,
        moneyReward: 7) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
        p.TakeDamage(ctx);
        engine.State.BattleData.Log($"{Name} ви нападна свирепо и нанесе {ctx.DamageTaken} щети!");
    }
}
