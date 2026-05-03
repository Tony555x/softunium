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
    public string Description { get; set; }
    public TargetType Target { get; set; }
    public int MpCost { get; set; }

    protected Skill(string name, string description, TargetType target, int mpCost)
    {
        Name = name;
        Description = description;
        Target = target;
        MpCost = mpCost;
    }

    // Returns a string which is the message to display.
    public abstract string Execute(Player player, List<Enemy> allEnemies, Enemy target);
}
