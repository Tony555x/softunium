using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class GuardSkill : Skill
{
    public override string Name => "Блок";
    public override string ShortDescription => "Намалява щетите от следващата атака.";
    public override string AccurateDescription => "Следващата атака срещу вас нанася -150% щети.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 4;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 2;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Defense };

    public GuardSkill()
    {
        Keywords.Add("percent");
        Keywords.Add("negative");
    }


    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new GuardStatus(1));
        return "Заемате защитна позиция.";
    }
}
