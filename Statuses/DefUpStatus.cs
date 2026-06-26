using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class DefUpStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Positive;
    internal override StatusCategory Category => StatusCategory.Stats;

    private readonly List<DebuffInstance> _instances = new();

    public DefUpStatus(int duration, float amount)
    {
        _instances.Add(new DebuffInstance(duration, amount));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is DefUpStatus defUp)
        {
            _instances.AddRange(defUp._instances);
        }
    }

    public override string GetDisplayString()
    {
        float total = _instances.Sum(i => i.Amount);
        if (total <= 0) return "";
        return $"[Защ +{(int)(total * 100)}%]";
    }

    public override string GetDescription()
    {
        float total = _instances.Sum(i => i.Amount);
        return $"Увеличава Защитата с {(int)(total * 100)}%.";
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
            statCtx.DefMult += totalIncrease;
        }
    }
}
