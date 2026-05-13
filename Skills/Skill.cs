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
        return keyword.ToLower() switch
        {
            "poison" or "отрова" => "Отрова: Нанася щети всеки ход. Стаковете намаляват с 1/3 всеки път (мин. 1).",
            "percent" or "процент" => "% бонуси за статистики/щети се събират: +50% Атака + +50% Атака = +100% Атака (не +125%).",
            "negative" => "Негативни % използват (1-1/x) формула: -50% Атака = 2/3 Атака. -100% Щети = 1/2 Щети. -200% = 1/3.",
            _ => ""
        };
    }

    // Returns a string which is the message to display.
    public abstract string Execute(Player player, List<Enemy> allEnemies, Enemy target);

    public virtual void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
    }
}
