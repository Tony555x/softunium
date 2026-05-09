using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class Programmer : Enemy
{
    private static readonly Random _rand = new();

    public Programmer() : base(
        name: "Програмист",
        maxHp: 35,
        attack: 12,
        defence: 8,
        speed: 6,
        magic: 8,
        wisdom: 2,
        luck: 6,
        xpReward: 10,
        moneyReward: 15) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        
        if (_rand.Next(100) < 30) // 30% chance to debuff
        {
            var ctx = this.PerformAttack(p, this.Attack / 2); // weaker hit
            p.Status.ApplyStatus(new AtkDownStatus(3, 0.15f));
            p.RecalcStats();
            engine.State.BattleData.Log($"{Name} изпрати объркващ код! Нанесе {ctx.DamageTaken} щети и намали атаката ви!");
        }
        else
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} използва Basic Attack и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
