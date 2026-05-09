using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class SmellyProgrammer : Enemy
{
    private static readonly Random _rand = new();

    public SmellyProgrammer() : base(
        name: "Смрадлив Програмист",
        maxHp: 30,
        attack: 10,
        defence: 7,
        speed: 8,
        magic: 10,
        wisdom: 0,
        luck: 3,
        xpReward: 12,
        moneyReward: 12) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        
        if (_rand.Next(100) < 35) // 35% chance to poison
        {
            var ctx = this.PerformAttack(p, this.Attack - 2); 
            p.Status.ApplyStatus(new PoisonStatus(2));
            engine.State.BattleData.Log($"{Name} хвърли мръсни чорапи! Нанесе {ctx.DamageTaken} щети и ви отрови (2 стака)!");
        }
        else
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} ви замери с клавиатура и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
