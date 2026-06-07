using System;
using Harduni.Core;

namespace Harduni.Enemies;

public class Kifla : Enemy
{
    private int _healCooldown = 0;

    public Kifla() : base(
        name: "Кифла",
        maxHp: 75, // 65 - 10% (6.5) -> ~58.5 -> round to mult of 5 -> 60
        attack: 13, // 15 - 2
        defence: 14, // 12 + 20% (2.4) -> 14
        speed: 6, // 6 - 1
        magic: 16,
        wisdom: 0,
        luck: 1,
        xpReward: 20,
        moneyReward: 3) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        if (_healCooldown > 0)
        {
            _healCooldown--;
        }
        
        if (_healCooldown == 0)
        {
            int totalHealed = 0;
            foreach (var enemy in engine.State.BattleData.Enemies)
            {
                if (enemy.Hp > 0 && enemy.Hp < enemy.MaxHp)
                {
                    var healCtx = enemy.Heal(this.Magic * 2);
                    totalHealed += healCtx.ActualHealed;
                }
            }
            if (totalHealed > 0)
            {
                _healCooldown = 3;
                engine.State.BattleData.Log($"{Name} оказа силна подкрепа и излекува съюзниците си за общо {totalHealed} Живот!");
                return;
            }
        }
        
        var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
        p.TakeDamage(ctx);
        engine.State.BattleData.Log($"{Name} удари шамар и нанесе {ctx.DamageTaken} щети!");
    }
}
