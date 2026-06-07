using Harduni.Models;

namespace Harduni.Statuses;

public class GuardStatus : Status
{
    private int _hitsLeft;

    public GuardStatus(int hits)
    {
        _hitsLeft = hits;
    }
    public override void OnStack(Status newStatus)
    {
        if (newStatus is GuardStatus def)
        {
            _hitsLeft += def._hitsLeft;
        }
    }

    public override string GetDisplayString()
    {
        return $"[Блок ({_hitsLeft})]";
    }

    public override string GetDescription()
    {
        return $"Намалява щетите от следващите {_hitsLeft} атаки със 150%.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.OnDamageTaken && ctx is DamageContext aCtx && aCtx.Type == DamageType.Attack)
        {
            aCtx.DamageMult -= 1.5f; 
            _hitsLeft--;
            if (_hitsLeft <= 0)
            {
                Destroy();
            }
        }
    }
}
