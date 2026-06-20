using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;
using Harduni.Core;

namespace Harduni.Skills;

public class Rot : Skill
{
    public override string Name => "Гниене";
    public override string ShortDescription => "Отравя всички врагове и активира отровата им два пъти.";
    public override string AccurateDescription => "Налага (Магия / 2) Отрова на всички врагове и активира отровата им два пъти.";
    public override TargetType Target => TargetType.Aoe;
    public override int MpCost => 0;
    public override int TempoCost => 5;
    public override bool IsTempoSkill => true;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Poison };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int poisonPotency = System.Math.Max(1, player.Magic / 2);
        var engine = player.GameState?.Engine;
        
        foreach (var enemy in allEnemies)
        {
            if (enemy.Hp > 0)
            {
                enemy.Status.ApplyStatus(new PoisonStatus(poisonPotency));
                
                var poison = enemy.Status.GetStatus<PoisonStatus>();
                if (poison != null && poison.Stacks > 0 && engine != null)
                {
                    // Trigger 1st time
                    poison.Trigger(engine);
                    
                    // Trigger 2nd time (if still alive and poison still active)
                    if (enemy.Hp > 0)
                    {
                        var poison2 = enemy.Status.GetStatus<PoisonStatus>();
                        if (poison2 != null && poison2.Stacks > 0)
                        {
                            poison2.Trigger(engine);
                        }
                    }
                }
            }
        }
        
        return $"нанесе отрова ({poisonPotency}) на всички врагове и активира отровата им два пъти.";
    }
}
