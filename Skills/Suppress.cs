using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Suppress : Skill
{
    public override string Name => "Потискане";
    public override string ShortDescription => "Атакува и намалява атаката на врага.";
    public override string AccurateDescription => "Атакува за (Атака) щети и намалява Атаката на врага с 25% за 3 хода.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 2;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack, SkillTag.Debuff };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        var ctx = new DamageContext(player, target, player.Attack, DamageType.Attack);
        target.TakeDamage(ctx);
        target.Status.ApplyStatus(new AtkDownStatus(3, 0.25f));
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети и намалихте атаката на {target.Name} с 25% за 3 хода.";
        if (ctx.IsLethal) msg = $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name}.";
        return msg;
    }
}
