using Harduni.Core;
using Harduni.Statuses;

namespace Harduni.Enemies;

public class Delta : Enemy
{
    private int _turnCount = 0;

    public Delta() : base(
        name: "Делта",
        maxHp: 70,
        attack: 16,
        defence: 12,
        speed: 6,
        magic: 10,
        wisdom: 0,
        luck: 1,
        xpReward: 16,
        moneyReward: 6) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;
        
        if (_turnCount % 4 == 1)
        {
            this.Status.ApplyStatus(new AtkUpStatus(4, 0.5f));
            this.RecalcStats();
            engine.State.BattleData.Log($"{Name} показа доминантност! Атаката му се увеличи драстично за 4 хода!");
        }
        else
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} ви удари със самочувствие и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
