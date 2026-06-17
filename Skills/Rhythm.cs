using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Rhythm : Skill
{
    public override string Name => "Ритъм";
    public override string ShortDescription => "Пасивно: +5% Атака и Магия за всяка точка Темпо.";
    public override string AccurateDescription => "Увеличава Атаката и Магията ви с 5% за всяка налична точка Темпо.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 0;
    public override int TempoCost => 0;
    public override bool IsTempoSkill => true; // Yes, it's a tempo skill
    public override bool UsableInBattle => false;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Passive };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        return "";
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.StatMult && ctx is StatModContext smc)
        {
            if (owner is Player player)
            {
                float bonus = player.Tempo * 0.05f;
                smc.AtkMult += bonus;
                smc.MagMult += bonus;
            }
        }
    }
}
