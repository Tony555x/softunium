using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class HolyLight : Skill
{
    public override string Name => "Свята Светлина";
    public override string ShortDescription => "Лекува и дава регенерация за тази битка.";
    public override string AccurateDescription => "Излекува ви за (Магия * 3) Живот и дава (Магия / 5) Регенерация за тази битка.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 12;
    public override int TempoCost => 5;
    public override bool IsTempoSkill => true;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        int healAmount = player.Magic * 3;
        var healCtx = player.Heal(healAmount);
        
        int regenAmount = System.Math.Max(1, player.Magic / 5);
        player.Status.ApplyStatus(new PersistentRegenStatus(1, regenAmount));
        
        return $"възстанови {healCtx.ActualHealed} Живот и получи {regenAmount} Регенерация за тази битка.";
    }
}
