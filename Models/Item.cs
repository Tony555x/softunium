using System;

namespace Harduni.Models;

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Action<Player> OnUse { get; set; }

    public Item(string name, string description, Action<Player> onUse)
    {
        Name = name;
        Description = description;
        OnUse = onUse;
    }
}
