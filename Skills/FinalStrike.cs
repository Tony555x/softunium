using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class FinalStrike : Skill
{
    public override string Name => "Завършващ удар";
    public override string ShortDescription => "Атакува с допълнителни щети спрямо липсващия живот на целта.";
    public override string AccurateDescription => "Атакува за (Атака) щети. Увеличава нанесените щети с процента на липсващия живот на врага.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 3;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        float missingHpPercent = target.MaxHp > 0 ? (float)(target.MaxHp - target.Hp) / target.MaxHp : 0f;
        
        var ctx = new DamageContext(player, target, player.Attack, DamageType.Attack);
        ctx.DamageMult += missingHpPercent;
        
        target.TakeDamage(ctx);
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети на {target.Name} (бонус +{(int)(missingHpPercent * 100)}%).";
        if (ctx.IsLethal) msg = $"Нанесохте фатален завършващ удар от {ctx.DamageTaken} щети на {target.Name}!";
        return msg;
    }
}
