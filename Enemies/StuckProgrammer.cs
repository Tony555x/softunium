using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class StuckProgrammer : Enemy
{
    private int _turnCount = 0;

    public StuckProgrammer() : base(
        name: "Забил Програмист",
        maxHp: 100,
        attack: 24,
        defence: 20,
        speed: 8,
        magic: 24,
        wisdom: 0,
        luck: 1,
        xpReward: 70,
        moneyReward: 40) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;
        int cycle = _turnCount % 4;

        if (cycle == 1) // Turn 1: Debuff
        {
            var ctx = this.PerformAttack(p, this.Attack - 4); 
            p.Status.ApplyStatus(new AtkDownStatus(3, 0.25f));
            p.Status.ApplyStatus(new SpdDownStatus(3, 0.15f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} предизвика Memory Leak! Намали Атаката и Скоростта ви и нанесе {ctx.DamageTaken} щети!");
        }
        else if (cycle == 2) // Turn 2: Poison
        {
            var ctx = this.PerformAttack(p, (int)(this.Attack * 0.5f)); 
            p.Status.ApplyStatus(new PoisonStatus(this.Magic));
            engine.State.BattleData.Log($"{Name} крашна и ви изпръска с токсични грешки! {ctx.DamageTaken} щети и Отрова ({this.Magic})!");
        }
        else // Turn 3 and 0 (4): Attack
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} използва груба сила и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
