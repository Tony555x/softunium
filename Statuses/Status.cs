using Harduni.Models;

namespace Harduni.Statuses;

public abstract class Status
{
    public Entity Owner { get; set; }

    public abstract void ProcessEvent(GameEvent ev, EventContext ctx);
    
    // Defines how the status behaves when an identical status (same type) is applied again.
    public abstract void OnStack(Status newStatus);

    public abstract string GetDisplayString();

    public virtual void Destroy()
    {
        Owner?.Status.RemoveStatus(this);
    }
}
