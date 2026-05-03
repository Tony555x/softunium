using System;
using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Core;

namespace Harduni.Locations;

public class Room
{
    public int EventType { get; set; } // 0 = event/empty, > 0 enemies
    public List<Enemy> EnemiesToSpawn { get; set; }
    public Func<List<Enemy>> EnemySpawner { get; set; }
    public IPanel EventInstance { get; set; }
    public bool IsCleared { get; set; } = false;

    public Room(int eventType, Func<List<Enemy>> enemySpawner = null, IPanel eventInstance = null)
    {
        EventType = eventType;
        EnemySpawner = enemySpawner;
        EventInstance = eventInstance;
        Reset();
    }

    public void Reset()
    {
        IsCleared = false;
        if (EnemySpawner != null)
        {
            EnemiesToSpawn = EnemySpawner();
        }
        else
        {
            EnemiesToSpawn = new List<Enemy>();
        }
    }
}
