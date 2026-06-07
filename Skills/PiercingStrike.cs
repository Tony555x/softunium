using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class PiercingStrike : Skill
{
    public override string Name => "Пробиващ удар";
    public override string ShortDescription => "Силна атака, която намалява защитата.";
    public override string AccurateDescription => "Атакува за (Атака * 1.5) щети и намалява Защитата на врага с 50% за 3 хода.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 6;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        var ctx = new DamageContext(player, target, (int)(player.Attack * 1.5), DamageType.Attack);
        target.TakeDamage(ctx);
        target.Status.ApplyStatus(new DefDownStatus(3, 0.5f));
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети на {target.Name} и пробихте защитата му.";
        if (ctx.IsLethal) msg = $"Нанесохте фатален пробиващ удар от {ctx.DamageTaken} щети на {target.Name}.";
        
        return msg;
    }
}
