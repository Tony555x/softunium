using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class AccelStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Positive;
    internal override StatusCategory Category => StatusCategory.Stats;

    private readonly List<DebuffInstance> _instances = new();

    public AccelStatus(int duration)
    {
        _instances.Add(new DebuffInstance(duration, 0.10f));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is AccelStatus other)
        {
            _instances.AddRange(other._instances);
        }
    }

    public override string GetDisplayString()
    {
        if (_instances.Count == 0) return "";
        int stacks = _instances.Count;
        return $"[Ускорение x{stacks}]";
    }

    public override string GetDescription()
    {
        int stacks = _instances.Count;
        return $"Увеличава Скоростта с {stacks * 50}%, но намалява Атаката, Защитата и Магията с {stacks * 10}%.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.EndTurn)
        {
            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                _instances[i].Duration--;
                if (_instances[i].Duration <= 0)
                {
                    _instances.RemoveAt(i);
                }
            }

            if (_instances.Count == 0)
            {
                Owner.RecalcStats();
                Destroy();
            }
        }
        else if (ev == GameEvent.StatMult && ctx is StatModContext statCtx)
        {
            float mult = _instances.Count;
            statCtx.SpdMult += mult * 0.50f;
            statCtx.AtkMult -= mult * 0.10f;
            statCtx.DefMult -= mult * 0.10f;
            statCtx.MagMult -= mult * 0.10f;
        }
    }
}
