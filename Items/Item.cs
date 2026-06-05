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
    public int MaxStacks { get; set; } = -1;
    public int Weight { get; set; } = 1;
    public int Value { get; set; } = 0;

    public Item(string name, string description, string accurateDescription, bool usableInBattle, bool usableOutsideBattle, int weight = 1, int maxStacks = -1)
    {
        Name = name;
        Description = description;
        AccurateDescription = accurateDescription;
        UsableInBattle = usableInBattle;
        UsableOutsideBattle = usableOutsideBattle;
        Weight = weight;
        MaxStacks = maxStacks;
    }

    public abstract string Use(Player player);
}
