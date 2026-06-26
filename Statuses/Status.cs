using Harduni.Models;
using Harduni.Core;
using System.Collections.Generic;

namespace Harduni.Statuses;

internal enum StatusPolarity
{
    None,
    Positive,
    Negative
}

internal enum StatusCategory
{
    None,
    Stats,
    Damaging,
    Healing
}

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

public abstract class Status
{
    public Entity Owner { get; set; }
    public bool IsPersistent { get; set; } = false;
    public List<string> Keywords { get; set; } = new();

    internal virtual StatusPolarity Polarity => StatusPolarity.None;
    internal virtual StatusCategory Category => StatusCategory.None;

    public abstract void ProcessEvent(GameEvent ev, EventContext ctx);
    
    // Defines how the status behaves when an identical status (same type) is applied again.
    public abstract void OnStack(Status newStatus);

    public abstract string GetDisplayString();
    public abstract string GetDescription();

    public virtual void Destroy()
    {
        Owner?.Status.RemoveStatus(this);
    }

    public virtual void Trigger(GameEngine engine)
    {
    }

    public virtual StatusSaveData Save()
    {
        return new StatusSaveData { Type = GetType().Name };
    }

    public virtual void Load(StatusSaveData data)
    {
    }
}
