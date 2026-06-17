using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class VampiricStrike : Skill
{
    public override string Name => "Вампирски Удар";
    public override string ShortDescription => "Атака и лечение.";
    public override string AccurateDescription => "Атакува един враг за (Атака * 2) щети и лекува за половината от нанесените щети.";
    public override TargetType Target => TargetType.Enemy;
    public override int MpCost => 8;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 4;
    public override List<SkillTag> Tags { get; } = new() { SkillTag.Attack, SkillTag.Healing };

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        var ctx = new DamageContext(player, target, player.Attack * 2, DamageType.Attack);
        target.TakeDamage(ctx);
        int baseHealAmount = ctx.DamageTaken / 2;
        var healCtx = player.Heal(baseHealAmount);

        string msg = $"Изсмукахте {healCtx.ActualHealed} живот от {target.Name} ({ctx.DamageTaken} щети).";
        if (ctx.IsLethal)
        {
            msg += " Врагът е мъртъв!";
        }
        return msg;
    }
}
