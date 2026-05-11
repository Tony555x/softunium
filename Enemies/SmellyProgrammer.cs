using System;
using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class SmellyProgrammer : Enemy
{
    private int _turnCount = 0;

    public SmellyProgrammer() : base(
        name: "Смрадлив Програмист",
        maxHp: 55,
        attack: 12,
        defence: 10,
        speed: 7,
        magic: 12,
        wisdom: 0,
        luck: 1,
        xpReward: 22,
        moneyReward: 12) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;
        
        if (_turnCount % 3 == 1)
        {
            int poisonStacks = this.Magic / 2;
            p.Status.ApplyStatus(new PoisonStatus(poisonStacks));
            engine.State.BattleData.Log($"{Name} хвърли мръсни чорапи и ви отрови ({poisonStacks} стака)");
        }
        else
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} ви замери с клавиатура и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
