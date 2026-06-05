using System;
using Harduni.Items;

namespace Harduni.Core;

public static class ItemFactory
{
    public static Item CreateItem(string name)
    {
        return name switch
        {
            "Баница" => new Banitsa(),
            "Вода" => new Water(),
            "Бонбони" => new Candy(),
            _ => null
        };
    }
}
