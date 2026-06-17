using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class MagicAffinity : Skill
{
    public override string Name => "Магически Афинитет";
    public override string ShortDescription => "Пасивно: +20% Магия.";
    public override string AccurateDescription => "Увеличава общата Магия с 20%.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 0;
    public override bool UsableInBattle => false;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Passive };

    public MagicAffinity()
    {
        Keywords.Add("percent");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        return "";
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.StatMult && ctx is StatModContext smc)
        {
            smc.MagMult += 0.20f;
        }
    }
}
