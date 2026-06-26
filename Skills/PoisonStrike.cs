using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class PoisonStrike : Skill
{
    public override string Name => "Отровен удар";
    public override string ShortDescription => "Атакува и нанася отрова.";
    public override string AccurateDescription => "Атакува за (Атака) щети и нанася отрова със сила (Магия / 2).";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 3;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack, SkillTag.Poison };

    public PoisonStrike()
    {
        Keywords.Add("poison");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        var ctx = new DamageContext(player, target, player.Attack, DamageType.Attack);
        target.TakeDamage(ctx);
        
        int poisonAmount = player.Magic / 2;
        target.Status.ApplyStatus(new PoisonStatus(poisonAmount));
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети и {poisonAmount} отрова на {target.Name}.";
        if (ctx.IsLethal) msg = $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name}.";
        
        return msg;
    }
}
