using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Rot : Skill
{
    public override string Name => "Гниене";
    public override string ShortDescription => "Отрови всички врагове и активира отровата им два пъти.";
    public override string AccurateDescription => "Налага (Магия / 2) Отрова на всички врагове и веднага активира техните отровни стакове два пъти.";
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
        
        foreach (var enemy in allEnemies)
        {
            if (enemy.Hp > 0)
            {
                enemy.Status.ApplyStatus(new PoisonStatus(poisonPotency));
                
                var poison = enemy.Status.GetStatus<PoisonStatus>();
                if (poison != null && poison.Stacks > 0)
                {
                    int dmg = poison.Stacks;
                    
                    // Trigger 1st time
                    enemy.Hp = System.Math.Max(0, enemy.Hp - dmg);
                    player.GameState?.BattleData.Log($"Отровата на {enemy.Name} се активира за {dmg} щети.");
                    
                    // Trigger 2nd time (if still alive)
                    if (enemy.Hp > 0)
                    {
                        enemy.Hp = System.Math.Max(0, enemy.Hp - dmg);
                        player.GameState?.BattleData.Log($"Отровата на {enemy.Name} се активира за втори път за {dmg} щети.");
                    }
                }
            }
        }
        
        return $"нанесе отрова ({poisonPotency}) на всички врагове и активира отровата им два пъти.";
    }
}
