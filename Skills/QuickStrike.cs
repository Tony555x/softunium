using System.Collections.Generic;
using System.Linq;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class QuickStrike : Skill
{
    public override string Name => "Бърз удар";
    public override string ShortDescription => "Атакува и намалява изчакването.";
    public override string AccurateDescription => "Атакува за (Атака * 0.8) щети и намалява най-дългото изчакване с 1.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 3;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        var ctx = new DamageContext(player, target, (int)(player.Attack * 0.8), DamageType.Attack);
        target.TakeDamage(ctx);
        
        var skillToReduce = player.EquippedSkills
            .Where(s => s.Cooldown > 0)
            .OrderByDescending(s => s.Cooldown)
            .FirstOrDefault();
            
        string cdMsg = "";
        if (skillToReduce != null)
        {
            skillToReduce.Cooldown--;
            cdMsg = $" Изчакването на {skillToReduce.Name} бе намалено.";
        }
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети на {target.Name}.{cdMsg}";
        if (ctx.IsLethal) msg = $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name}.{cdMsg}";
        
        return msg;
    }
}
