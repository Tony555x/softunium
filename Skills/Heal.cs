using System;
using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Heal : Skill
{
    public override string Name => "Лечение";
    public override string ShortDescription => "Възстановява малко живот.";
    public override string AccurateDescription => "Възстановява 30 Живот.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 4;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => true;
    public override int BaseCooldown => 1; // Example cooldown


    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int healAmount = 30;
        var ctx = player.Heal(healAmount);
        return $"Възстановихте {ctx.ActualHealed} Живот.";
    }
}
