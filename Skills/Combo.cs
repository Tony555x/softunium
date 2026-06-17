using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Combo : Skill
{
    public override string Name => "Комбо";
    public override string ShortDescription => "Атакува една цел 6 пъти.";
    public override string AccurateDescription => "Атакува една цел 6 пъти, всяка атака нанася щети равни на (Атака * 0.75). Защитата се прилага на всеки удар.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 0;
    public override int TempoCost => 5;
    public override bool IsTempoSkill => true;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        if (target == null) return "Няма цел.";

        int totalDamage = 0;
        int hits = 0;
        bool wasLethal = false;

        for (int i = 0; i < 6; i++)
        {
            if (target.Hp <= 0) break;
            
            var ctx = new DamageContext(player, target, (int)(player.Attack * 0.75), DamageType.Attack);
            target.TakeDamage(ctx);
            totalDamage += ctx.DamageTaken;
            hits++;

            if (ctx.IsLethal)
            {
                wasLethal = true;
            }
        }

        string msg = $"Нанесохте общо {totalDamage} щети на {target.Name} с {hits} удара.";
        if (wasLethal)
        {
            msg = $"Нанесохте фатален комбо удар от общо {totalDamage} щети на {target.Name} ({hits} удара).";
        }
        return msg;
    }
}
