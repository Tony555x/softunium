using Harduni.Models;
using System.Collections.Generic;

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

public abstract class Status
{
    public Entity Owner { get; set; }
    public bool IsPersistent { get; set; } = false;
    public List<string> Keywords { get; set; } = new();

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
}
