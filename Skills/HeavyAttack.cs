using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class HeavyAttack : Skill
{
    public HeavyAttack() : base("Тежък Удар", "Силна единична атака.", "Атакува един враг за (Атака + 15) щети.", TargetType.Enemy, 3, true, false) { }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        var ctx = player.PerformAttack(target, player.Attack + 15);
        if (ctx.IsLethal)
        {
            return $"Нанесохте тежък фатален удар от {ctx.DamageTaken} щети на {target.Name}!";
        }
        return $"Нанесохте {ctx.DamageTaken} щети на {target.Name}.";
    }
}
