using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class Dizainerka : Enemy
{
    private int _turnCount = 0;

    public Dizainerka() : base(
        name: "Дизайнерка",
        maxHp: 500,
        attack: 30,
        defence: 50,
        speed: 7,
        magic: 42,
        wisdom: 0,
        luck: 1,
        xpReward: 150,
        moneyReward: 10) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;
        int cycle = _turnCount % 3;

        if (cycle == 1) // Turn 1
        {
            this.Status.ApplyStatus(new GuardStatus(3));
            int totalHealed = 0;
            foreach (var enemy in engine.State.BattleData.Enemies)
            {
                if (enemy.Hp > 0)
                {
                    var healCtx = enemy.Heal(this.Magic * 4);
                    totalHealed += healCtx.ActualHealed;
                    if (enemy != this)
                    {
                        enemy.Status.ApplyStatus(new GuardStatus(2));
                        enemy.RecalcStats();
                    }
                }
            }
            engine.State.BattleData.Log($"{Name} редизайнна формацията си, получи 3 Блок, даде 2 Блок на съюзниците си и излекува всички съюзници за общо {totalHealed}!");
        }
        else // Turn 2 and 3
        {
            var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
            p.TakeDamage(ctx);
            p.Status.ApplyStatus(new DefDownStatus(20, 0.05f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} те застреля с молив, нанесе ти {ctx.DamageTaken} щети и намали защитата ти!");
        }
    }
}
