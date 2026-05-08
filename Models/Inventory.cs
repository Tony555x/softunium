using System;
using System.Collections.Generic;
using Harduni.Items;

namespace Harduni.Models;

public class Inventory
{
    private readonly List<Item> _items = new();
    public IReadOnlyList<Item> Items => _items;
    public int Count => _items.Count;

    public Item this[int index] => _items[index];

    public void AddItem(Item item)
    {
        var existing = _items.Find(i => i.Name == item.Name);
        
        if (existing != null)
        {
            if (existing.MaxStacks == -1)
            {
                existing.Amount += item.Amount;
            }
            else
            {
                int space = existing.MaxStacks - existing.Amount;
                int toAdd = System.Math.Min(space, item.Amount);
                existing.Amount += toAdd;
                
                // If there's still leftover and we want to support multiple stacks of same item:
                // For now, the existing logic in Player.cs didn't seem to support multiple stacks of the same item type,
                // it just capped it. I'll stick to that logic unless told otherwise.
            }
        }
        else
        {
            if (item.MaxStacks != -1)
            {
                item.Amount = System.Math.Min(item.Amount, item.MaxStacks);
            }
            _items.Add(item);
        }
    }

    public void RemoveItem(Item item)
    {
        item.Amount--;
        if (item.Amount <= 0)
        {
            _items.Remove(item);
        }
    }

    public void Clear()
    {
        _items.Clear();
    }
}
