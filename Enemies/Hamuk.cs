using Harduni.Core;
using Harduni.Statuses;
using Harduni.Models;

namespace Harduni.Enemies;

public class Hamuk : Enemy
{
    private int _turnCount = 0;

    public Hamuk() : base(
        name: "Хамук",
        maxHp: 450,
        attack: 45,
        defence: 40,
        speed: 6,
        magic: 10,
        wisdom: 0,
        luck: 1,
        xpReward: 140,
        moneyReward: 20) { }

    public override void TakeAction(GameEngine engine)
    {
        var p = engine.State.Player;
        _turnCount++;

        if ((_turnCount - 1) % 4 == 0)
        {
            foreach (var enemy in engine.State.BattleData.Enemies)
            {
                if (enemy.Hp > 0)
                {
                    enemy.Status.ApplyStatus(new AtkUpStatus(3, 1.00f));
                    enemy.RecalcStats();
                }
            }
            engine.State.BattleData.Log($"{Name} нададе мощен рев и увеличи атаката на всички съюзници със 100%!");
        }
        else
        {
            var ctx = new DamageContext(this, p, this.Attack, DamageType.Attack);
            p.TakeDamage(ctx);
            engine.State.BattleData.Log($"{Name} нанесе {ctx.DamageTaken} щети с огромната си сила!");
        }
    }
}
