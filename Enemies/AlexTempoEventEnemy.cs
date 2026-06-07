using System;
using Harduni.Core;

namespace Harduni.Enemies;

public class AlexTempoEventEnemy : Enemy
{
    public AlexTempoEventEnemy() : base(
        name: "Алекс",
        maxHp: 350,
        attack: 1,
        defence: 34,
        speed: 40, // 8 * 5
        magic: 36,
        wisdom: 0,
        luck: 1,
        xpReward: 0,
        moneyReward: 0) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        int totalDamage = 0;
        for (int i = 0; i < 3; i++)
        {
            var ctx = new DamageContext(this, p, 1, DamageType.Attack);
            p.TakeDamage(ctx);
            totalDamage += ctx.DamageTaken;
        }
        engine.State.BattleData.Log($"{Name} атакува бързо 3 пъти и нанесе общо {totalDamage} щети!");
    }
}
