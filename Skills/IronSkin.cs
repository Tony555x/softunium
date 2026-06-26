using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class IronSkin : Skill
{
    public override string Name => "Желязна кожа";
    public override string ShortDescription => "Пасивно: +20% Защита.";
    public override string AccurateDescription => "Увеличава общата Защита с 20%.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 0;
    public override bool UsableInBattle => false;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Defense, SkillTag.Passive };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        return "";
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.StatMult && ctx is StatModContext smc)
        {
            smc.DefMult += 0.20f;
        }
    }
}
