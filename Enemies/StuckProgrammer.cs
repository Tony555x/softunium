using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class StuckProgrammer : Enemy
{
    private int _turnCount = 0;

    public StuckProgrammer() : base(
        name: "Забил Програмист",
        maxHp: 350,
        attack: 36,
        defence: 34,
        speed: 8,
        magic: 36,
        wisdom: 0,
        luck: 1,
        xpReward: 150,
        moneyReward: 10) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;
        int cycle = _turnCount % 4;

        if (cycle == 1) // Turn 1: Debuff
        {
            var ctx = new DamageContext(this, p, this.Attack - 4, DamageType.Attack);
            p.TakeDamage(ctx);
            p.Status.ApplyStatus(new AtkDownStatus(4, 0.75f));
            p.Status.ApplyStatus(new SpdDownStatus(4, 0.25f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} предизвика Memory Leak! Намали Атаката и Скоростта ви и нанесе {ctx.DamageTaken} щети!");
        }
        else if (cycle == 2) // Turn 2: Poison
        {
            var ctx = new DamageContext(this, p, (int)(this.Attack * 0.8f), DamageType.Attack);
            p.TakeDamage(ctx);
            p.Status.ApplyStatus(new PoisonStatus(this.Magic));
            engine.State.BattleData.Log($"{Name} крашна и ви изпръска с токсични грешки! {ctx.DamageTaken} щети и Отрова ({this.Magic})!");
        }
        else // Turn 3 and 0 (4): Attack
        {
            var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
            p.TakeDamage(ctx);
            engine.State.BattleData.Log($"{Name} с груба сила нанесе {ctx.DamageTaken} щети!");
        }
    }
}
