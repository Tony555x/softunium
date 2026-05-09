using Harduni.Core;

namespace Harduni.Enemies;

public class OligofrenBoss : Enemy
{
    private int _turnCount = 0;

    public OligofrenBoss() : base(
        name: "Олигофрен (БОС)",
        maxHp: 100,
        attack: 15,
        defence: 10,
        speed: 5,
        magic: 15,
        wisdom: 10,
        luck: 15,
        xpReward: 40,
        moneyReward: 100) { }

    public override void TakeAction(GameEngine engine)
    {
        _turnCount++;
        var p = engine.State.Player;

        if (_turnCount % 4 == 3)
        {
            engine.State.BattleData.Log($"{Name} подготвя силна атака...");
        }
        else if(_turnCount % 4 == 0){
            var ctx = this.PerformAttack(p, this.Attack * 2);
            engine.State.BattleData.Log($"{Name} избухна и ти нанесе {ctx.DamageTaken} щети!");
        }
        else
        {
            var ctx = this.PerformAttack(p, this.Attack);
            engine.State.BattleData.Log($"{Name} ви атакува и нанесе {ctx.DamageTaken} щети!");
        }
    }
}
