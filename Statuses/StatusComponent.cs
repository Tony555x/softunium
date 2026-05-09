using System.Collections.Generic;
using System.Linq;

using Harduni.Models;

namespace Harduni.Statuses;

public class StatusComponent
{
    private readonly List<Status> _statuses = new();
    public Entity Owner { get; }

    public StatusComponent(Entity owner)
    {
        Owner = owner;
    }

    public IReadOnlyList<Status> Statuses => _statuses;

    public void ApplyStatus(Status newStatus)
    {
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

    public T GetStatus<T>() where T : Status
    {
        return _statuses.OfType<T>().FirstOrDefault();
    }

    public void RemoveStatus(Status status)
    {
        _statuses.Remove(status);
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
    }
}
