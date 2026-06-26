using Harduni.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Harduni.Statuses;

public class PersistentDefStatus : Status
{
    internal override StatusPolarity Polarity
    {
        get
        {
            float total = _instances.Sum(i => i.Amount);
            if (total > 0) return StatusPolarity.Positive;
            if (total < 0) return StatusPolarity.Negative;
            return StatusPolarity.None;
        }
    }
    internal override StatusCategory Category => StatusCategory.Stats;

    private class Instance
    {
        public int Fights;
        public float Amount;
        public Instance(int fights, float amount) { Fights = fights; Amount = amount; }
    }

    private readonly List<Instance> _instances = new();

    public PersistentDefStatus(int fights, float amount)
    {
        IsPersistent = true;
        _instances.Add(new Instance(fights, amount));
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is PersistentDefStatus other)
        {
            _instances.AddRange(other._instances);
        }
    }

    public override string GetDisplayString()
    {
        float total = _instances.Sum(i => i.Amount);
        if (total == 0) return "";
        string sign = total > 0 ? "+" : "";
        return $"[Защ {sign}{(int)(total * 100)}%]";
    }

    public override string GetDescription()
    {
        float total = _instances.Sum(i => i.Amount);
        string action = total > 0 ? "Увеличава" : "Намалява";
        return $"{action} Защитата с {(int)Math.Abs(total * 100)}% (Персистентно).";
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
        else if (ev == GameEvent.StatMult && ctx is StatModContext statCtx)
        {
            float total = _instances.Sum(i => i.Amount);
            statCtx.DefMult += total;
        }
    }
}
