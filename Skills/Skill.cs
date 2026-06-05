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
    public virtual string Name { get; set; } = "Skill";
    public virtual string ShortDescription { get; set; } = "";
    public virtual string AccurateDescription { get; set; } = "";
    public virtual TargetType Target { get; set; } = TargetType.Enemy;
    public virtual int MpCost { get; set; } = 0;
    public virtual bool UsableInBattle { get; set; } = true;
    public virtual bool UsableOutsideBattle { get; set; } = false;
    public List<string> Keywords { get; set; } = new();

    public int Cooldown { get; set; }
    public virtual int BaseCooldown { get; set; } = 0;

    protected Skill()
    {
    }

    public virtual bool CanPlay(bool inBattle)
    {
        if (inBattle)
        {
            return UsableInBattle && Cooldown == 0;
        }
        return UsableOutsideBattle;
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
