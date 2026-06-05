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
    public override int MpCost => 5;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;

    public PoisonStrike()
    {
        Keywords.Add("poison");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        var ctx = player.PerformAttack(target, player.Attack);
        
        int poisonAmount = player.Magic / 2;
        target.Status.ApplyStatus(new PoisonStatus(poisonAmount));
        
        string msg = $"Нанесохте {ctx.DamageTaken} щети и {poisonAmount} отрова на {target.Name}.";
        if (ctx.IsLethal) msg = $"Нанесохте фатален удар от {ctx.DamageTaken} щети на {target.Name}.";
        
        return msg;
    }
}
