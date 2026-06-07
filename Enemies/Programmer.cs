using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class Programmer : Enemy
{
    private int _turnCount = 0;

    public Programmer() : base(
        name: "Програмист",
        maxHp: 65,
        attack: 15,
        defence: 12,
        speed: 6,
        magic: 10,
        wisdom: 0,
        luck: 1,
        xpReward: 20,
        moneyReward: 3) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;
        
        if (_turnCount % 3 == 1)
        {
            var ctx = new DamageContext(this, p, this.Attack - 3, DamageType.Attack); // weaker hit
            p.TakeDamage(ctx);
            p.Status.ApplyStatus(new AtkDownStatus(3, 0.5f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} изпрати объркващ код! Нанесе {ctx.DamageTaken} щети и намали атаката ви!");
        }
        else
        {
            var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
            p.TakeDamage(ctx);
            engine.State.BattleData.Log($"{Name} нанесе {ctx.DamageTaken} щети с основна атака!");
        }
    }
}
