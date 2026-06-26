using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Exertion : Skill
{
    public override string Name => "Напрежение";
    public override string ShortDescription => "Лекува живот спрямо липсващия айрян.";
    public override string AccurateDescription => "Възстановява Живот, равен на липсващия Айрян.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 4;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => true;
    public override int BaseCooldown => 1;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Healing };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int mpMissing = player.MaxMp - player.Mp;
        var ctx = player.Heal(mpMissing);
        return $"Възстановихте {ctx.ActualHealed} Живот.";
    }
}
