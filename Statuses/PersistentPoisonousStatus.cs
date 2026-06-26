using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class PersistentPoisonousStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Positive;
    internal override StatusCategory Category => StatusCategory.None;

    private readonly List<PersistentStatusInstance> _instances = new();

    public PersistentPoisonousStatus(int fights, int potency)
    {
        IsPersistent = true;
        _instances.Add(new PersistentStatusInstance(fights, potency));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is PersistentPoisonousStatus other)
        {
            _instances.AddRange(other._instances);
        }
    }

    public override string GetDisplayString()
    {
        int total = _instances.Sum(i => i.Potency);
        return $"[Отровен {total}]";
    }

    public override string GetDescription()
    {
        int total = _instances.Sum(i => i.Potency);
        return $"Нанася {total} отрова на врага при всяка атака.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.CombatEnd)
        {
            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                _instances[i].Fights--;
                if (_instances[i].Fights <= 0)
                {
                    _instances.RemoveAt(i);
                }
            }

            if (_instances.Count == 0)
            {
                Destroy();
            }
        }
        else if (ev == GameEvent.OnDamageDealt && ctx is DamageContext aCtx && aCtx.Type == DamageType.Attack)
        {
            if (aCtx.Attacker == Owner)
            {
                int total = _instances.Sum(i => i.Potency);
                aCtx.Target.Status.ApplyStatus(new PoisonStatus(total));
            }
        }
    }
}
