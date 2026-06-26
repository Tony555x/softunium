using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class NigredoStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Negative;
    internal override StatusCategory Category => StatusCategory.Stats;

    private readonly List<DebuffInstance> _instances = new();

    public NigredoStatus(int duration)
    {
        _instances.Add(new DebuffInstance(duration, 0.50f));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is NigredoStatus other)
        {
            _instances.AddRange(other._instances);
        }
    }

    public override string GetDisplayString()
    {
        if (_instances.Count == 0) return "";
        int stacks = _instances.Count;
        return $"[Нигредо x{stacks}]";
    }

    public override string GetDescription()
    {
        int stacks = _instances.Count;
        return $"Намалява Атаката, Защитата и Магията с {stacks * 50}%.";
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
            float total = _instances.Sum(i => i.Amount);
            statCtx.AtkMult -= total;
            statCtx.DefMult -= total;
            statCtx.MagMult -= total;
        }
    }
}
