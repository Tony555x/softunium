using Harduni.Models;
using Harduni.Core;

namespace Harduni.Statuses;

public class PersistentObservedStatus : Status
{
    public PersistentObservedStatus()
    {
        IsPersistent = true;
    }

    internal override StatusPolarity Polarity => StatusPolarity.Negative;
    internal override StatusCategory Category => StatusCategory.Stats;

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.CalculateMpCost && ctx is MpCostContext mpCtx)
        {
            mpCtx.CostAdd += 1;
        }
    }

    public override void OnStack(Status newStatus)
    {
        // Does not stack
    }

    public override string GetDisplayString()
    {
        return "[НАБЛЮДАВАН]";
    }

    public override string GetDescription()
    {
        return "Персистентно: Всички умения струват +1 Айрян.";
    }
}
