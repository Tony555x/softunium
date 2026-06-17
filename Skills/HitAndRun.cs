using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class HitAndRun : Skill
{
    public override string Name => "Удар и отстъп";
    public override string ShortDescription => "Атакува и увеличава защитата.";
    public override string AccurateDescription => "Атакува за (Атака) щети и дава +25% Защита за 3 хода.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 4;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack, SkillTag.Defense, SkillTag.Buff };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        var ctx = new DamageContext(player, target, player.Attack, DamageType.Attack);
        target.TakeDamage(ctx);
        player.Status.ApplyStatus(new DefUpStatus(3, 0.25f), DelayedTurn.Next);
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети на {target.Name} и засилихте защитата си.";
        if (ctx.IsLethal) msg = $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name} и засилихте защитата си.";
        
        return msg;
    }
}
