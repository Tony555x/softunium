using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class StuckProgrammer : Enemy
{
    private static readonly Random _rand = new();

    public StuckProgrammer() : base(
        name: "Забил Програмист",
        maxHp: 65,
        attack: 16,
        defence: 12,
        speed: 10,
        magic: 15,
        wisdom: 5,
        luck: 5,
        xpReward: 35,
        moneyReward: 40) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        int action = _rand.Next(100);

        if (action < 25) // 25% poison
        {
            var ctx = this.PerformAttack(p, this.Attack - 3); 
            p.Status.ApplyStatus(new PoisonStatus(3));
            engine.State.BattleData.Log($"{Name} крашна и ви изпръска с токсични грешки! {ctx.DamageTaken} щети и Отрова (3)!");
        }
        else if (action < 50) // 25% debuffs
        {
            var ctx = this.PerformAttack(p, this.Attack - 4); 
            p.Status.ApplyStatus(new AtkDownStatus(3, 0.20f));
            p.Status.ApplyStatus(new SpdDownStatus(3, 0.20f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} предизвика Memory Leak! Намали Атаката и Скоростта ви и нанесе {ctx.DamageTaken} щети!");
        }
        else // 50% basic
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} използва груба сила и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
