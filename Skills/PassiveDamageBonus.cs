using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class PassiveDamageBonus : Skill
{
    public override string Name => "Сила на духа";
    public override string ShortDescription => "Пасивно: +20% Щети.";
    public override string AccurateDescription => "Увеличава нанесените щети с 20%.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 0;
    public override bool UsableInBattle => false;
    public override bool UsableOutsideBattle => false;

    public PassiveDamageBonus()
    {
        Keywords.Add("percent");
    }


    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        return ""; // Passive
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.OnAttack && ctx is AttackContext aCtx)
        {
            aCtx.DamageMult += 0.20f;
        }
    }

}
