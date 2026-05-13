using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Cleave : Skill
{
    public Cleave() : base("Разсичане", "Атакува всички врагове.", "Нанася (Атака * 0.8) щети на всички врагове.", TargetType.Aoe, 4, true, false) { }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int totalDamage = 0;
        int baseAoeDamage = (int)(player.Attack * 0.8);
        
        foreach(var e in allEnemies)
        {
            if (e.Hp > 0)
            {
                var ctx = player.PerformAttack(e, baseAoeDamage);
                totalDamage += ctx.DamageTaken;
            }
        }
        
        return $"Нанесохте общо {totalDamage} щети на враговете.";
    }
}
