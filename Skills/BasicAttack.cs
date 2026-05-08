using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class BasicAttack : Skill
{
    public BasicAttack() : base("Атака", "Стандартна атака.", "Нанася (Атака) щети на избрания враг.", TargetType.Enemy, 0, true, false) { }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        var ctx = player.PerformAttack(target, player.Attack);
        if (ctx.IsLethal)
        {
            return $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name}!";
        }
        return $"Нанесохте {ctx.DamageTaken} щети на {target.Name}.";
    }
}
