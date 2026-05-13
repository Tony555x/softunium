using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Focus : Skill
{
    public Focus() : base("Фокус", "Увеличава атаката за следващите 3 хода.", "+50% Атака за следващите 3 хода.", TargetType.Self, 4, true, false) 
    {
        Keywords.Add("percent");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new AtkUpStatus(3, 0.5f), DelayedTurn.Next);
        
        return "Подготвяте се за атака!";
    }
}
