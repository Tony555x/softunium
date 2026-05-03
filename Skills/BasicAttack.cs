using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class BasicAttack : Skill
{
    public BasicAttack() : base("Основна Атака", "Нанася щети на един враг, базирани на вашата Атака.", TargetType.Enemy, 0) { }

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
