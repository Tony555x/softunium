using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Cleave : Skill
{
    public override string Name => "Разсичане";
    public override string ShortDescription => "Атакува всички врагове.";
    public override string AccurateDescription => "Атакува всички врагове за (Атака) щети.";
    public override TargetType Target => TargetType.Aoe;
    public override int MpCost => 2;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack };


    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int totalDamage = 0;
        int baseAoeDamage = player.Attack;
        
        foreach(var e in allEnemies)
        {
            if (e.Hp > 0)
            {
                var ctx = new DamageContext(player, e, baseAoeDamage, DamageType.Attack);
                e.TakeDamage(ctx);
                totalDamage += ctx.DamageTaken;
            }
        }
        
        return $"Нанесохте общо {totalDamage} щети на враговете.";
    }
}
