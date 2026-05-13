using System;
using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Heal : Skill
{
    public Heal() : base("Лечение", "Възстановява малко живот.", "Възстановява 30 Живот.", TargetType.Self, 4, true, true) { }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int healAmount = 30;
        var ctx = player.Heal(healAmount);
        return $"Възстановихте {ctx.ActualHealed} Живот.";
    }
}
