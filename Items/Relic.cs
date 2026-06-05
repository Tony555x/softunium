using System.Collections.Generic;
using Harduni.Models;

namespace Harduni.Items;

public abstract class Relic
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string AccurateDescription { get; set; }

    protected Relic(string name, string description, string accurateDescription)
    {
        Name = name;
        Description = description;
        AccurateDescription = accurateDescription;
    }

    public virtual void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
    }
}
