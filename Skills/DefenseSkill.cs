using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class DefenseSkill : Skill
{
    public DefenseSkill() : base("Защита", "Намалява щетите от следващата атака.", "Следващата атака срещу вас нанася -100% щети.", TargetType.Self, 4, true, false) 
    {
        Keywords.Add("percent");
        Keywords.Add("negative");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new DefenseStatus(1));
        return "Заемате защитна позиция.";
    }
}
