using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class HeavyStrike : Skill
{
    public override string Name => "Тежък Удар";
    public override string ShortDescription => "Силна единична атака.";
    public override string AccurateDescription => "Атакува един враг за (Атака + 15) щети.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 2;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        var ctx = new DamageContext(player, target, player.Attack + 15, DamageType.Attack);
        target.TakeDamage(ctx);
        if (ctx.IsLethal)
        {
            return $"Нанесохте тежък фатален удар от {ctx.DamageTaken} щети на {target.Name}!";
        }
        return $"Нанесохте {ctx.DamageTaken} щети на {target.Name}.";
    }
}
