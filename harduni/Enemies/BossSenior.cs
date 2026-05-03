using Harduni.Core;

namespace Harduni.Enemies;

public class BossSenior : Enemy
{
    private int _turnCount = 0;

    public BossSenior() : base("Главен Разработчик (БОС)", 100, 15, 10, 15, 10, 5, 15, 150) { }

    public override void TakeAction(GameEngine engine)
    {
        _turnCount++;
        var p = engine.State.Player;

        if (_turnCount % 3 == 0)
        {
            var ctx = this.PerformAttack(p, this.Attack * 2);
            engine.State.BattleData.BattleMessage = $"{Name} ИЗПОЛЗВА УЛТИМАТИВНА АТАКА и нанесе {ctx.DamageTaken} щети!";
        }
        else
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.BattleMessage = $"{Name} ви атакува и нанесе {ctx.DamageTaken} щети!";
        }
    }
}
