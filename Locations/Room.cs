using System;
using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Core;

namespace Harduni.Locations;

public class Room
{
    public int EventType { get; set; } // 0 = event/empty, > 0 enemies
    public List<Enemy>? EnemiesToSpawn { get; set; }
    public IPanel? EventInstance { get; set; }
    public float LootMultiplier { get; set; } = 1.0f;
    public bool IsCleared { get; set; } = false;

    public Room(int eventType, List<Enemy>? enemiesToSpawn = null, IPanel? eventInstance = null)
    {
        EventType = eventType;
        EnemiesToSpawn = enemiesToSpawn ?? new List<Enemy>();
        EventInstance = eventInstance;
    }
}
