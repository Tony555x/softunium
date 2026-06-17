using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Filth : Skill
{
    public override string Name => "Гадост";
    public override string ShortDescription => "Нанася отрова.";
    public override string AccurateDescription => "Нанася отрова със сила (Магия).";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 5;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 2;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Poison };

    public Filth()
    {
        Keywords.Add("poison");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        int poisonAmount = (int)(player.Magic);
        target.Status.ApplyStatus(new PoisonStatus(poisonAmount));
        
        return $"Нанесохте {poisonAmount} отрова на {target.Name}.";
    }
}
