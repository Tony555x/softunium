using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Models;

namespace Harduni.Skills;

public enum TargetType
{
    Enemy,
    Aoe,
    Self
}

public abstract class Skill
{
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string AccurateDescription { get; set; }
    public TargetType Target { get; set; }
    public int MpCost { get; set; }
    public bool UsableInBattle { get; set; }
    public bool UsableOutsideBattle { get; set; }
    public List<string> Keywords { get; set; } = new();

    protected Skill(string name, string shortDesc, string accurateDesc, TargetType target, int mpCost, bool usableInBattle = true, bool usableOutsideBattle = false)
    {
        Name = name;
        ShortDescription = shortDesc;
        AccurateDescription = accurateDesc;
        Target = target;
        MpCost = mpCost;
        UsableInBattle = usableInBattle;
        UsableOutsideBattle = usableOutsideBattle;
    }

    public static string GetKeywordExplanation(string keyword)
    {
        return SkillKeywords.GetExplanation(keyword);
    }

    // Returns a string which is the message to display.
    public abstract string Execute(Player player, List<Enemy> allEnemies, Enemy target);

    public virtual void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
    }
}
