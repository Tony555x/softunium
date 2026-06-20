using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Nigredo : Skill
{
    public override string Name => "Нигредо";
    public override string ShortDescription => "Намалява Атаката, Защитата и Магията на всички врагове с 50% за 5 хода.";
    public override string AccurateDescription => "Намалява Атаката, Защитата и Магията на всички врагове с 50% за 5 хода.";
    public override TargetType Target => TargetType.Aoe;
    public override int MpCost => 0;
    public override int TempoCost => 5;
    public override bool IsTempoSkill => true;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Debuff };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        foreach (var enemy in allEnemies)
        {
            if (enemy.Hp > 0)
            {
                enemy.Status.ApplyStatus(new NigredoStatus(5));
            }
        }
        return "активира Нигредо! Атаката, Защитата и Магията на всички врагове са намалени с 50% за 5 хода.";
    }
}
