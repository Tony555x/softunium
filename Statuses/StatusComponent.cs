using System.Collections.Generic;
using System.Linq;
using Harduni.Models;

namespace Harduni.Statuses;

public enum DelayedTurn { This, Next, Next2, Next3 }

public class StatusComponent
{
    private readonly List<Status> _statuses = new();
    private readonly Dictionary<DelayedTurn, List<Status>> _delayedStatuses = new()
    {
        { DelayedTurn.Next, new List<Status>() },
        { DelayedTurn.Next2, new List<Status>() },
        { DelayedTurn.Next3, new List<Status>() }
    };

    public Entity Owner { get; }

    public StatusComponent(Entity owner)
    {
        Owner = owner;
    }

    public IReadOnlyList<Status> Statuses => _statuses;

    public void ApplyStatus(Status newStatus)
    {
        bool isInDungeon = Harduni.Program.Engine?.State?.DungeonData?.IsInDungeon == true;
        if (!isInDungeon)
        {
            return;
        }

        bool inBattle = Harduni.Program.Engine?.CurrentPanel == Harduni.Program.Engine?.State?.World?.BattlePanel;
        if (!inBattle && !newStatus.IsPersistent)
        {
            return;
        }

        newStatus.Owner = this.Owner;
        
        var existingType = newStatus.GetType();
        var existingStatus = _statuses.FirstOrDefault(s => s.GetType() == existingType);

        if (existingStatus != null)
        {
            existingStatus.OnStack(newStatus);
        }
        else
        {
            _statuses.Add(newStatus);
        }
    }

    public void LoadStatus(Status status)
    {
        status.Owner = this.Owner;
        var existingType = status.GetType();
        var existingStatus = _statuses.FirstOrDefault(s => s.GetType() == existingType);
        if (existingStatus != null)
        {
            existingStatus.OnStack(status);
        }
        else
        {
            _statuses.Add(status);
        }
    }

    public void ApplyStatus(Status newStatus, DelayedTurn delay)
    {
        if (delay == DelayedTurn.This)
        {
            ApplyStatus(newStatus);
        }
        else
        {
            bool isInDungeon = Harduni.Program.Engine?.State?.DungeonData?.IsInDungeon == true;
            if (!isInDungeon)
            {
                return;
            }

            bool inBattle = Harduni.Program.Engine?.CurrentPanel == Harduni.Program.Engine?.State?.World?.BattlePanel;
            if (!inBattle && !newStatus.IsPersistent)
            {
                return;
            }
            _delayedStatuses[delay].Add(newStatus);
        }
    }

    public T GetStatus<T>() where T : Status
    {
        return _statuses.OfType<T>().FirstOrDefault();
    }

    public void RemoveStatus(Status status)
    {
        _statuses.Remove(status);
    }

    public void ClearNonPersistent()
    {
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            if (!_statuses[i].IsPersistent)
            {
                _statuses[i].Destroy();
            }
        }
        foreach (var list in _delayedStatuses.Values) list.Clear();
    }

    public void ClearAll()
    {
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            _statuses[i].Destroy();
        }
        foreach (var list in _delayedStatuses.Values) list.Clear();
    }

    public string GetCombinedDisplayString()
    {
        if (_statuses.Count == 0) return "";
        var strings = _statuses.Select(s => s.GetDisplayString()).Where(s => !string.IsNullOrEmpty(s));
        return string.Join(" ", strings);
    }

    public void TriggerEvent(GameEvent ev, EventContext ctx)
    {
        // Iterate backwards or on a copy in case a status removes itself during processing
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            _statuses[i].ProcessEvent(ev, ctx);
        }

        if (ev == GameEvent.StartTurn)
        {
            // 1. Apply all statuses scheduled for "Next" (which is now "This")
            bool changed = _delayedStatuses[DelayedTurn.Next].Count > 0;
            foreach (var s in _delayedStatuses[DelayedTurn.Next])
            {
                ApplyStatus(s);
            }
            _delayedStatuses[DelayedTurn.Next].Clear();

            // 2. Shift others
            _delayedStatuses[DelayedTurn.Next].AddRange(_delayedStatuses[DelayedTurn.Next2]);
            _delayedStatuses[DelayedTurn.Next2].Clear();

            _delayedStatuses[DelayedTurn.Next2].AddRange(_delayedStatuses[DelayedTurn.Next3]);
            _delayedStatuses[DelayedTurn.Next3].Clear();

            if (changed)
            {
                Owner.RecalcStats();
            }
        }
    }
}
