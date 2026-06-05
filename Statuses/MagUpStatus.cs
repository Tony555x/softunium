using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class MagUpStatus : Status
{
    private readonly List<DebuffInstance> _instances = new();

    public MagUpStatus(int duration, float amount)
    {
        _instances.Add(new DebuffInstance(duration, amount));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is MagUpStatus magUp)
        {
            _instances.AddRange(magUp._instances);
        }
    }

    public override string GetDisplayString()
    {
        float total = _instances.Sum(i => i.Amount);
        if (total <= 0) return "";
        return $"[Маг +{(int)(total * 100)}%]";
    }

    public override string GetDescription()
    {
        float total = _instances.Sum(i => i.Amount);
        return $"Увеличава Магията с {(int)(total * 100)}%.";
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
            float totalIncrease = _instances.Sum(i => i.Amount);
            statCtx.MagMult += totalIncrease;
        }
    }
}
