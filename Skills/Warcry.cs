using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Warcry : Skill
{
    public override string Name => "Боен вик";
    public override string ShortDescription => "Увеличава атаката за следващите 3 хода.";
    public override string AccurateDescription => "+75% Атака за следващите 3 хода.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 3;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Buff };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new AtkUpStatus(3, 0.75f), DelayedTurn.Next);
        
        return "Надавате мощен боен вик!";
    }
}
