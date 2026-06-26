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

    public class DefInstance
    {
        public int Fights { get; set; }
        public float Amount { get; set; }
        public DefInstance(int fights, float amount) { Fights = fights; Amount = amount; }
    }

    private readonly List<DefInstance> _instances = new();
    public List<DefInstance> Instances => _instances;

    public PersistentDefStatus()
    {
        IsPersistent = true;
    }

    public PersistentDefStatus(int fights, float amount)
    {
        IsPersistent = true;
        _instances.Add(new DefInstance(fights, amount));
    }

    public override StatusSaveData Save()
    {
        var data = base.Save();
        data.Instances = _instances.Select(i => new StatusInstanceSaveData
        {
            Fights = i.Fights,
            Amount = i.Amount
        }).ToList();
        return data;
    }

    public override void Load(StatusSaveData data)
    {
        _instances.Clear();
        if (data.Instances != null)
        {
            foreach (var inst in data.Instances)
            {
                _instances.Add(new DefInstance(inst.Fights, inst.Amount ?? 0f));
            }
        }
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
