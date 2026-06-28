using System.Collections.Generic;
using System.Linq;
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
    
    public virtual int GetMpCost(Player player)
    {
        var ctx = new MpCostContext(MpCost);
        player.TriggerEvent(GameEvent.CalculateMpCost, ctx);
        return ctx.GetTotalCost();
    }
    
    public virtual int TempoCost { get; set; } = 0;
    public virtual bool IsTempoSkill { get; set; } = false;
    public virtual bool UsableInBattle { get; set; } = true;
    public virtual bool UsableOutsideBattle { get; set; } = false;
    public List<string> Keywords { get; set; } = new();
    public virtual List<SkillTag> Tags { get; } = new();

    public int Cooldown { get; set; }
    public virtual int BaseCooldown { get; set; } = 0;

    protected Skill()
    {
    }

    public bool HasTag(SkillTag tag) => Tags.Contains(tag);

    public virtual string GetDetailedDescription()
    {
        string tagsText = Tags.Count > 0 ? "Тагове: " + string.Join(", ", Tags.Select(t => t.GetBulgarianName())) + "\n" : "";
        string info = tagsText + AccurateDescription;
        foreach (var kw in Keywords)
        {
            string explanation = GetKeywordExplanation(kw);
            if (!string.IsNullOrEmpty(explanation))
            {
                info += "\n" + explanation;
            }
        }
        return info;
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
