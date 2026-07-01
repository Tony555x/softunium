using Harduni.Models;
using Harduni.Skills;

namespace Harduni.Relics;

public class FlashDrive : Relic
{
    public Skill MarkedSkill { get; private set; }

    public FlashDrive() : base(
        name: "10 TB флашка",
        description: "Съдържа много информация и един маркиран файл.",
        accurateDescription: "Последно използваното умение изисква 1 по-малко Айрян. В момента маркирано: (null)."
    )
    {
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.OnSkillUsed && ctx is SkillUsedContext skillCtx)
        {
            MarkedSkill = skillCtx.Skill;
            AccurateDescription = $"Последно използваното умение изисква 1 по-малко Айрян. В момента маркирано: {MarkedSkill.Name}.";
        }
        else if (ev == GameEvent.CalculateMpCost && ctx is MpCostContext mpCtx)
        {
            if (MarkedSkill != null && mpCtx.Skill != null && mpCtx.Skill.GetType() == MarkedSkill.GetType())
            {
                mpCtx.CostAdd -= 1;
            }
        }
    }
}
