using Harduni.Skills;

namespace Harduni.Models;

public class SkillUsedContext : EventContext
{
    public Skill Skill { get; }

    public SkillUsedContext(Skill skill)
    {
        Skill = skill;
    }
}
