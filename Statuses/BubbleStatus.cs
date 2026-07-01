using Harduni.Models;
using Harduni.Core;
using System;

namespace Harduni.Statuses;

public class BubbleStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Positive;
    internal override StatusCategory Category => StatusCategory.None;

    public int Stacks { get; set; }

    public BubbleStatus(int stacks)
    {
        Stacks = Math.Min(stacks, 5);
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is BubbleStatus p)
        {
            Stacks = Math.Min(Stacks + p.Stacks, 5);
        }
    }

    public override string GetDisplayString()
    {
        return $"[Балончета {Stacks}]";
    }

    public override string GetDescription()
    {
        return $"Увеличава нанесените щети с {Stacks * 25}%. След използване на [Атака] умение, губите всички балончета.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.OnDamageDealt && ctx is DamageContext aCtx && aCtx.Attacker == Owner && aCtx.Type == DamageType.Attack)
        {
            aCtx.DamageMult += 0.25f * Stacks;
        }
    }
}
