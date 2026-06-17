using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class BasicAttack : Skill
{
    public override string Name => "Атака";
    public override string ShortDescription => "Стандартна атака.";
    public override string AccurateDescription => "Атакува един враг за (Атака) щети.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 0;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack };


    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        var ctx = new DamageContext(player, target, player.Attack, DamageType.Attack);
        target.TakeDamage(ctx);
        if (ctx.IsLethal)
        {
            return $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name}!";
        }
        return $"Нанесохте {ctx.DamageTaken} щети на {target.Name}.";
    }
}
