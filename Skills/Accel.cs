using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Accel : Skill
{
    public override string Name => "Ускорение";
    public override string ShortDescription => "Дава +50% Скорост, но -10% Атака, Защита и Магия за 10 хода.";
    public override string AccurateDescription => "Увеличава Скоростта ви с 50%, но намалява Атаката, Защитата и Магията ви с 10%. Ефектът трае 10 хода и може да се наслагва адитивно.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 5;
    public override int TempoCost => 5;
    public override bool IsTempoSkill => true;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new AccelStatus(10));
        return "активира Ускорение! (+50% Скорост, -10% Атака, Защита и Магия за 10 хода)";
    }
}
