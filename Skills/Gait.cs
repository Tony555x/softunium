using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class Gait : Skill
{
    private bool _isActive = false;

    public override string Name => "Походка";
    public override string ShortDescription => "Пасивно: +1 Скорост. Активно: +2 Скорост за битката.";
    public override string AccurateDescription => "Пасивно: Дава +1 Скорост докато е екипирано. Активно (веднъж на битка): Дава допълнителни +2 Скорост до края на битката.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 2;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 0;

    public override bool CanPlay(bool inBattle)
    {
        return base.CanPlay(inBattle) && !_isActive;
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        _isActive = true;
        player.RecalcStats();
        return "Използвахте Походка! Скоростта ви се увеличи.";
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        base.ProcessEvent(owner, ev, ctx);
        
        if (ev == GameEvent.StatAdd && ctx is StatModContext smc)
        {
            smc.SpdAdd += 1; // Passive
            if (_isActive)
            {
                smc.SpdAdd += 2; // Active
            }
        }
        else if (ev == GameEvent.CombatEnd)
        {
            _isActive = false;
        }
    }
}
