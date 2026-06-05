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

    public int TotalWeight
    {
        get
        {
            int total = 0;
            foreach (var item in _items)
            {
                total += item.Weight * item.Amount;
            }
            return total;
        }
    }

    public bool AddItem(Item item, int maxWeight)
    {
        int weightToAdd = item.Weight * item.Amount;
        if (TotalWeight + weightToAdd > maxWeight)
        {
            return false;
        }

        var existing = _items.Find(i => i.Name == item.Name);
        if (existing != null)
        {
            existing.Amount += item.Amount;
        }
        else
        {
            _items.Add(item);
        }
        return true;
    }

    // Keep parameterless AddItem for backward compatibility (e.g. from SaveManager) or default to large weight
    public void AddItem(Item item)
    {
        AddItem(item, int.MaxValue);
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
