using System.Collections.Generic;

namespace Harduni.Models;

public class SaveData
{
    public PlayerSaveData Player { get; set; }
    public Dictionary<string, string> Flags { get; set; }
    public DungeonSaveData Dungeon { get; set; }
}

public class ItemSaveData
{
    public string Name { get; set; }
    public int Amount { get; set; }
}

public class PlayerSaveData
{
    public string Name { get; set; }
    public string BattleName { get; set; }
    public int Level { get; set; }
    public int Xp { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Mp { get; set; }
    public int MaxMp { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }
    public int Speed { get; set; }
    public int Magic { get; set; }
    public int Wisdom { get; set; }
    public int Luck { get; set; }
    public int Alignment { get; set; }
    public int Money { get; set; }
    public List<ItemSaveData> Inventory { get; set; }
    public List<string> Skills { get; set; }
    public List<string> EquippedSkills { get; set; }
}

public class DungeonSaveData
{
    public string CurrentLocationName { get; set; }
    public int CurrentRoomIndex { get; set; }
    public List<bool> ClearedRooms { get; set; }
}
