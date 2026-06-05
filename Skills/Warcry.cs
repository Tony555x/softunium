using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Warcry : Skill
{
    public override string Name => "Боен вик";
    public override string ShortDescription => "Увеличава атаката за следващите 3 хода.";
    public override string AccurateDescription => "+50% Атака за следващите 3 хода.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 4;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;

    public Warcry()
    {
        Keywords.Add("percent");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new AtkUpStatus(3, 0.5f), DelayedTurn.Next);
        
        return "Надавате мощен боен вик!";
    }
}
