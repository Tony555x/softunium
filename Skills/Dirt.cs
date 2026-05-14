using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Dirt : Skill
{
    public Dirt() : base("Мръсотия", "Нанася отрова.", "Нанася отрова със сила (Магия * 2 / 3).", TargetType.Enemy, 4, true, false) 
    {
        Keywords.Add("poison");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";
        
        int poisonAmount = (int)(player.Magic * 2 / 3);
        target.Status.ApplyStatus(new PoisonStatus(poisonAmount));
        
        return $"Нанесохте {poisonAmount} отрова на {target.Name}.";
    }
}
