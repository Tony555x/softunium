using System;
using Harduni.Core;
using Harduni.Statuses;
using Harduni.Models;


namespace Harduni.Enemies;

public class Zubarka : Enemy
{
    private int _turnCount = 0;

    public Zubarka() : base(
        name: "Зубърка",
        maxHp: 60, // 65 - 10% (6.5) -> ~58.5 -> round to mult of 5 -> 60
        attack: 12, // 15 - 2
        defence: 14, // 12 + 20% (2.4) -> 14
        speed: 5, // 6 - 1
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
            this.Status.ApplyStatus(new GuardStatus(2));
            engine.State.BattleData.Log($"{Name} се разсея да зубри и получи 2 Блок!");
        }
        else
        {
            var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
            p.TakeDamage(ctx);
            p.Status.ApplyStatus(new DefDownStatus(10, 0.05f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} хвърли книга, нанесе {ctx.DamageTaken} щети и леко намали защитата ти!");
        }
    }
}
