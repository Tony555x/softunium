using System;

namespace Harduni.Skills;

public enum SkillTag
{
    Attack,
    Poison,
    Healing,
    Defense,
    Buff,
    Debuff,
    Passive
}

public static class SkillTagExtensions
{
    public static string GetBulgarianName(this SkillTag tag)
    {
        return tag switch
        {
            SkillTag.Attack => "[Атака]",
            SkillTag.Poison => "[Отрова]",
            SkillTag.Healing => "[Лечение]",
            SkillTag.Defense => "[Защита]",
            SkillTag.Buff => "[Усилване]",
            SkillTag.Debuff => "[Отслабване]",
            SkillTag.Passive => "[Пасивно]",
            _ => tag.ToString()
        };
    }
}
