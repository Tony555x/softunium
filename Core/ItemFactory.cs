using System;
using Harduni.Items;

namespace Harduni.Core;

public static class ItemFactory
{
    public static Item CreateItem(string name)
    {
        return name switch
        {
            "Малка отвара" => new SmallPotion(),
            _ => null
        };
    }
}
