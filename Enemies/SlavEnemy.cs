using Harduni.Core;

namespace Harduni.Enemies;

public class SlavEnemy : Enemy
{
    public SlavEnemy() : base(
        name: "Слав",
        maxHp: 9999,
        attack: 500,
        defence: 500,
        speed: 100,
        magic: 500,
        wisdom: 500,
        luck: 500,
        xpReward: 9999,
        moneyReward: 9999) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        var ctx = this.PerformAttack(p, this.Attack);
        engine.State.BattleData.Log($"{Name} ███████?██?██████  и нанася {ctx.DamageTaken} щети!");
    }
}
