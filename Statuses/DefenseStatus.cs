using Harduni.Models;

namespace Harduni.Statuses;

public class DefenseStatus : Status
{
    private int _hitsLeft;

    public DefenseStatus(int hits)
    {
        _hitsLeft = hits;
    }
    public override void OnStack(Status newStatus)
    {
        if (newStatus is DefenseStatus def)
        {
            _hitsLeft += def._hitsLeft;
        }
    }

    public override string GetDisplayString()
    {
        return $"[Защита ({_hitsLeft})]";
    }

    public override string GetDescription()
    {
        return $"Намалява щетите от следващите {_hitsLeft} атаки със 100%.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.OnAttacked && ctx is AttackContext aCtx)
        {
            aCtx.DamageMult -= 1.0f; // This results in 0.5x damage in the additive system
            _hitsLeft--;
            if (_hitsLeft <= 0)
            {
                Destroy();
            }
        }
    }
}
