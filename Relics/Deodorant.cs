using Harduni.Models;
using Harduni.Statuses;

namespace Harduni.Relics;

public class Deodorant : Relic
{
    public Deodorant() : base(
        name: "Дезодорант",
        description: "Мирише на евтино, но поне не мирише на пот.",
        accurateDescription: "След като бъдете атакуван, налага -25% атака за 4 хода на атакуващия."
    )
    {
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.AfterAttacked && ctx is DamageContext dmgCtx && dmgCtx.Attacker != null)
        {
            // Inflict -25% attack for 4 turns to attacker
            dmgCtx.Attacker.Status.ApplyStatus(new AtkDownStatus(4, 0.25f));
            dmgCtx.Attacker.RecalcStats();
        }
    }
}
