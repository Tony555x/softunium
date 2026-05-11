using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class SpdDownStatus : Status
{
    private readonly List<DebuffInstance> _instances = new();

    public SpdDownStatus(int duration, float amount)
    {
        _instances.Add(new DebuffInstance(duration, amount));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is SpdDownStatus spdDown)
        {
            _instances.AddRange(spdDown._instances);
        }
    }

    public override string GetDisplayString()
    {
        float total = _instances.Sum(i => i.Amount);
        if (total <= 0) return "";
        return $"[Скр -{(int)(total * 100)}%]";
    }

    public override string GetDescription()
    {
        float total = _instances.Sum(i => i.Amount);
        return $"Намалява Скоростта с {(int)(total * 100)}%.";
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
            float totalReduction = _instances.Sum(i => i.Amount);
            statCtx.SpdMult -= totalReduction;
        }
    }
}
