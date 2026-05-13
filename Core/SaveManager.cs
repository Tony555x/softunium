using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Harduni.Models;
using Harduni.Skills;
using Harduni.Locations;

namespace Harduni.Core;

public static class SaveManager
{
    private static string GetFilePath(int slot) => $"save_slot_{slot}.json";

    public static bool SaveExists(int slot) => File.Exists(GetFilePath(slot));

    public static void Save(int slot, GameEngine engine)
    {
        var p = engine.State.Player;
        var dungeon = engine.State.DungeonData;

        var data = new SaveData
        {
            Flags = engine.State.Flags,
            Player = new PlayerSaveData
            {
                Name = p.Name,
                BattleName = p.BattleName,
                Level = p.Level,
                Xp = p.Xp,
                Hp = p.Hp,
                MaxHp = p.MaxHp,
                Mp = p.Mp,
                MaxMp = p.MaxMp,
                Attack = p.Attack,
                Defence = p.Defence,
                Speed = p.Speed,
                Magic = p.Magic,
                Wisdom = p.Wisdom,
                Luck = p.Luck,
                Alignment = p.Alignment,
                Money = p.Money,
                Inventory = p.Inventory.Items.Select(i => new ItemSaveData { Name = i.Name, Amount = i.Amount }).ToList(),
                Skills = p.Skills.Select(s => s.Name).ToList()
            },
            Dungeon = new DungeonSaveData
            {
                CurrentLocationName = engine.State.LastLocationPanel?.GetType().Name ?? "",
                CurrentRoomIndex = dungeon.CurrentRoomIndex,
                ClearedRooms = dungeon.Rooms.Select(r => r.IsCleared).ToList()
            }
        };

        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GetFilePath(slot), json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving: {ex.Message}");
        }
    }

    public static void Load(int slot, GameEngine engine)
    {
        if (!SaveExists(slot)) return;

        try
        {
            string json = File.ReadAllText(GetFilePath(slot));
            var data = JsonSerializer.Deserialize<SaveData>(json);
            if (data == null) return;

            var p = engine.State.Player;
            p.Name = data.Player.Name;
            p.BattleName = data.Player.BattleName;
            p.Level = data.Player.Level;
            p.RecalculateBaseStats();
            
            p.Xp = data.Player.Xp;
            p.Hp = data.Player.Hp;
            p.Mp = data.Player.Mp;
            
            p.Alignment = data.Player.Alignment;
            p.Money = data.Player.Money;

            p.Inventory.Clear();
            foreach (var itemData in data.Player.Inventory)
            {
                var item = ItemFactory.CreateItem(itemData.Name);
                if (item != null)
                {
                    item.Amount = itemData.Amount;
                    p.Inventory.AddItem(item);
                }
            }

            p.Skills.Clear();
            foreach (var skillName in data.Player.Skills)
            {
                var skill = CreateSkill(skillName);
                if (skill != null) p.Skills.Add(skill);
            }

            engine.State.Flags = data.Flags ?? new Dictionary<string, string>();
            
            // Restore Location
            RestoreLocation(data.Dungeon.CurrentLocationName, engine);
            
            engine.State.DungeonData.CurrentRoomIndex = data.Dungeon.CurrentRoomIndex;
            if (data.Dungeon.ClearedRooms != null)
            {
                for (int i = 0; i < data.Dungeon.ClearedRooms.Count && i < engine.State.DungeonData.Rooms.Count; i++)
                {
                    engine.State.DungeonData.Rooms[i].IsCleared = data.Dungeon.ClearedRooms[i];
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading: {ex.Message}");
        }
    }

    private static void RestoreLocation(string panelName, GameEngine engine)
    {
        var world = engine.State.World;
        IPanel targetPanel = panelName switch
        {
            "WisdomRoomLocation" => world.WisdomRoom,
            "WisdomDungeon" => world.WisdomDungeon,
            "ProgressRoomLocation" => world.ProgressRoom,
            "ProgressDungeon" => world.ProgressDungeon,
            "TeamworkRoomLocation" => world.TeamworkRoom,
            "TeamworkDungeon" => world.TeamworkDungeon,
            "IntegrityRoomLocation" => world.IntegrityRoom,
            "IntegrityDungeon" => world.IntegrityDungeon,
            "KordorLocation" => world.Kordor,
            _ => null
        };

        if (targetPanel != null)
        {
            if (targetPanel is Dungeon dungeon)
            {
                dungeon.Enter(engine);
            }
            else
            {
                engine.ChangeRootPanel(targetPanel);
            }
        }
    }

    private static Skill CreateSkill(string name)
    {
        return name switch
        {
            "Тежък Удар" => new HeavyAttack(),
            "Лечение" => new Heal(),
            "Разсичане" => new Cleave(),
            "Фокус" => new Focus(),
            "Сила на духа" => new PassiveAtkBonus(),
            "Мръсотия" => new Dirt(),
            "Защита" => new DefenseSkill(),
            _ => null
        };
    }
}
