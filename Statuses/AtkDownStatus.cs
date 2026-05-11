using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class DebuffInstance
{
    public int Duration { get; set; }
    public float Amount { get; set; }

    public DebuffInstance(int duration, float amount)
    {
        Duration = duration;
        Amount = amount;
    }
}

public class AtkDownStatus : Status
{
    private readonly List<DebuffInstance> _instances = new();

    public AtkDownStatus(int duration, float amount)
    {
        _instances.Add(new DebuffInstance(duration, amount));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is AtkDownStatus atkDown)
        {
            _instances.AddRange(atkDown._instances);
        }
    }

    public override string GetDisplayString()
    {
        float total = _instances.Sum(i => i.Amount);
        if (total <= 0) return "";
        return $"[Атк -{(int)(total * 100)}%]";
    }

    public override string GetDescription()
    {
        float total = _instances.Sum(i => i.Amount);
        return $"Намалява Атаката с {(int)(total * 100)}%.";
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
            statCtx.AtkMult -= totalReduction;
        }
    }
}
