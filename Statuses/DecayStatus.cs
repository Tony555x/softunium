using Harduni.Models;
using System;

namespace Harduni.Statuses;

public class DecayStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Negative;
    internal override StatusCategory Category => StatusCategory.Damaging;

    public int Stacks { get; set; }

    public DecayStatus(int stacks)
    {
        IsPersistent = true;
        Stacks = stacks;
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is DecayStatus other)
        {
            Stacks += other.Stacks;
        }
    }

    public override string GetDisplayString()
    {
        return $"[Разграждане {Stacks}]";
    }

    public override string GetDescription()
    {
        return $"Нанася {Stacks} щети в края на хода. Не намалява и остава след битка.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.EndTurn && ctx is TurnContext tCtx)
        {
            var battleData = tCtx.Engine.State.BattleData;
            
            int dmg = Stacks;
            var dmgCtx = new DamageContext(null, Owner, dmg, DamageType.Decay);
            Owner.TakeDamage(dmgCtx);

            battleData.Log($"{Owner.Name} поема {dmgCtx.DamageTaken} щети от разграждане.");
        }
    }
}
