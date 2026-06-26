using Harduni.Models;
using System.Collections.Generic;
using System.Linq;

namespace Harduni.Statuses;

public class PersistentStatusInstance
{
    public int Fights { get; set; }
    public int Potency { get; set; }

    public PersistentStatusInstance(int fights, int potency)
    {
        Fights = fights;
        Potency = potency;
    }
}

public class PersistentRegenStatus : Status
{
    internal override StatusPolarity Polarity => StatusPolarity.Positive;
    internal override StatusCategory Category => StatusCategory.Healing;

    private readonly List<PersistentStatusInstance> _instances = new();
    public List<PersistentStatusInstance> Instances => _instances;

    public PersistentRegenStatus()
    {
        IsPersistent = true;
    }

    public PersistentRegenStatus(int fights, int potency)
    {
        IsPersistent = true;
        _instances.Add(new PersistentStatusInstance(fights, potency));
    }

    public override StatusSaveData Save()
    {
        var data = base.Save();
        data.Instances = _instances.Select(i => new StatusInstanceSaveData
        {
            Fights = i.Fights,
            Potency = i.Potency
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
                _instances.Add(new PersistentStatusInstance(inst.Fights, inst.Potency ?? 0));
            }
        }
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is PersistentRegenStatus other)
        {
            _instances.AddRange(other._instances);
        }
    }

    public override string GetDisplayString()
    {
        int total = _instances.Sum(i => i.Potency);
        return $"[Реген {total}]";
    }

    public override string GetDescription()
    {
        int total = _instances.Sum(i => i.Potency);
        return $"Възстановява {total} живот в края на всеки ход.";
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
        else if (ev == GameEvent.EndTurn && ctx is TurnContext tCtx)
        {
            int total = _instances.Sum(i => i.Potency);
            Owner.Heal(total);
            tCtx.Engine.State.BattleData.Log($"{Owner.Name} се регенерира за {total} живот.");
        }
    }
}
