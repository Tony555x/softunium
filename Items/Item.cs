using System;
using Harduni.Models;

namespace Harduni.Items;

public abstract class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string AccurateDescription { get; set; }
    public bool UsableInBattle { get; set; }
    public bool UsableOutsideBattle { get; set; }
    public int Amount { get; set; } = 1;
    public int MaxStacks { get; set; } = 1;

    public Item(string name, string description, string accurateDescription, bool usableInBattle, bool usableOutsideBattle, int maxStacks = 1)
    {
        Name = name;
        Description = description;
        AccurateDescription = accurateDescription;
        UsableInBattle = usableInBattle;
        UsableOutsideBattle = usableOutsideBattle;
        MaxStacks = maxStacks;
    }

    public abstract string Use(Player player);
}
