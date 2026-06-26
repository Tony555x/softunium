using System.Collections.Generic;
using System.Linq;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Step : Skill
{
    public override string Name => "Стъпка";
    public override string ShortDescription => "Лекува и намалява изчакването на друго умение.";
    public override string AccurateDescription => "Възстановява (Магия * 0.2) Живот и намалява най-дългото изчакване на друго екипирано умение с 1.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 2;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => true;
    public override int BaseCooldown => 1;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Healing };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int healAmount = (int)(player.Magic * 0.2f);
        var healCtx = player.Heal(healAmount);
        
        var skillToReduce = player.EquippedSkills
            .Where(s => s != this && s.Cooldown > 0)
            .OrderByDescending(s => s.Cooldown)
            .FirstOrDefault();

        string cdMsg = "";
        if (skillToReduce != null)
        {
            skillToReduce.Cooldown--;
            cdMsg = $" Изчакването на {skillToReduce.Name} бе намалено.";
        }

        return $"Използвахте Стъпка: възстановихте {healCtx.ActualHealed} Живот.{cdMsg}";
    }
}
