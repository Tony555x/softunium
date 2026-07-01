using Harduni.Models;
using Harduni.Statuses;
using Harduni.Skills;

namespace Harduni.Relics;

public class BubbleGun : Relic
{
    public BubbleGun() : base(
        name: "Пистолет за балончета",
        description: "Струваше 10 стотинки от ТЕМУ.",
        accurateDescription: "При използване на умение без таг [Атака], получавате 1 Балонче (макс 5). Всяко балонче дава +25% щети. При използване на умение с таг [Атака], губите всички балончета. В момента балончета: 0."
    )
    {
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.OnSkillUsed && ctx is SkillUsedContext skillCtx && owner is Player player)
        {
            if (skillCtx.Skill.HasTag(SkillTag.Attack))
            {
                var bubble = player.Status.GetStatus<BubbleStatus>();
                bubble?.Destroy();
                AccurateDescription = "При използване на умение без таг [Атака], получавате 1 Балонче (макс 5). Всяко балонче дава +25% щети. При използване на умение с таг [Атака], губите всички балончета. В момента балончета: 0.";
            }
            else
            {
                player.Status.ApplyStatus(new BubbleStatus(1));
                var bubble = player.Status.GetStatus<BubbleStatus>();
                int stacks = bubble?.Stacks ?? 1;
                AccurateDescription = $"При използване на умение без таг [Атака], получавате 1 Балонче (макс 5). Всяко балонче дава +25% щети. При използване на умение с таг [Атака], губите всички балончета. В момента балончета: {stacks}.";
            }
        }
    }
}
