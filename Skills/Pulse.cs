using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Pulse : Skill
{
    public override string Name => "Пулс";
    public override string ShortDescription => "Лекува и отслабва всички врагове.";
    public override string AccurateDescription => "Възстановява (Магия) Живот и намалява Атаката на всички врагове с 50% за 3 хода.";
    public override TargetType Target => TargetType.Self; // Effectively AoE debuff + self heal
    public override int MpCost => 8;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 3;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Healing, SkillTag.Debuff };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        var healCtx = player.Heal(player.Magic);
        
        foreach (var enemy in allEnemies)
        {
            if (enemy.Hp > 0)
            {
                enemy.Status.ApplyStatus(new AtkDownStatus(3, 0.5f));
            }
        }
        
        return $"Използвахте Пулс: възстановихте {healCtx.ActualHealed} Живот и отслабихте всички врагове.";
    }
}
