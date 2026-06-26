using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Harduni.Models;
using Harduni.Skills;
using Harduni.Locations;
using Harduni.Statuses;

namespace Harduni.Core;

public static class SaveManager
{
    private static string GetFilePath(int slot) => $"save_slot_{slot}.json";

    public static bool SaveExists(int slot) => File.Exists(GetFilePath(slot));

    public static (int Level, string? TimeSaved)? GetSaveMetadata(int slot)
    {
        if (!SaveExists(slot)) return null;
        try
        {
            string json = File.ReadAllText(GetFilePath(slot));
            var data = JsonSerializer.Deserialize<SaveData>(json);
            if (data?.Player != null)
            {
                return (data.Player.Level, data.TimeSaved);
            }
        }
        catch { }
        return null;
    }

    public static void Save(int slot, GameEngine engine)
    {
        var p = engine.State.Player;
        var dungeon = engine.State.DungeonData;

        var data = new SaveData
        {
            Flags = engine.State.Flags,
            TimeSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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
                Skills = p.Skills.Select(s => s.Name).ToList(),
                EquippedSkills = p.EquippedSkills.Select(s => s.Name).ToList(),
                PersistentStatuses = p.Status.Statuses.Where(s => s.IsPersistent).Select(s => s.Save()).ToList()
            },
            Dungeon = new DungeonSaveData
            {
                CurrentLocationName = engine.State.LastLocationPanel?.GetType().Name ?? "",
                CurrentRoomIndex = dungeon.CurrentRoomIndex,
                ClearedRooms = dungeon.Rooms.Select(r => r.IsCleared).ToList(),
                IsInDungeon = dungeon.IsInDungeon
            }
        };

        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GetFilePath(slot), json);
        }
        catch (Exception ex)
        {
            VConsole.WriteLine($"Error saving: {ex.Message}");
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

            p.EquippedSkills.Clear();
            if (data.Player.EquippedSkills != null)
            {
                foreach (var skillName in data.Player.EquippedSkills)
                {
                    // Find the skill instance from the already loaded Skills list to ensure reference equality
                    var skill = p.Skills.Find(s => s.Name == skillName);
                    if (skill != null && !p.EquippedSkills.Contains(skill)) p.EquippedSkills.Add(skill);
                }
            }

            engine.State.Flags = data.Flags ?? new Dictionary<string, string>();
            
            // Restore Location
            RestoreLocation(data.Dungeon.CurrentLocationName, engine);
            
            engine.State.DungeonData.CurrentRoomIndex = data.Dungeon.CurrentRoomIndex;
            engine.State.DungeonData.IsInDungeon = data.Dungeon.IsInDungeon;
            if (data.Dungeon.ClearedRooms != null)
            {
                for (int i = 0; i < data.Dungeon.ClearedRooms.Count && i < engine.State.DungeonData.Rooms.Count; i++)
                {
                    engine.State.DungeonData.Rooms[i].IsCleared = data.Dungeon.ClearedRooms[i];
                }
            }

            p.Status.ClearAll();
            if (data.Player.PersistentStatuses != null)
            {
                foreach (var sData in data.Player.PersistentStatuses)
                {
                    var status = StatusRouter.CreateStatus(sData.Type);
                    if (status != null)
                    {
                        status.Load(sData);
                        p.Status.LoadStatus(status);
                    }
                }
            }

            p.RecalcStats();
        }
        catch (Exception ex)
        {
            VConsole.WriteLine($"Error loading: {ex.Message}");
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
            "LeadershipRoomLocation" => world.LeadershipRoom,
            "LeadershipDungeon" => world.LeadershipDungeon,

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
            "Тежък Удар" => new HeavyStrike(),
            "Лечение" => new Heal(),
            "Разсичане" => new Cleave(),
            "Фокус" => new Warcry(),
            "Боен вик" => new Warcry(),
            "Сила на духа" => new PassiveDamageBonus(),
            "Мръсотия" => new Filth(),
            "Гадост" => new Filth(),
            "Концентрация" => new Concentration(),
            "Желязна кожа" => new IronSkin(),
            "Отровен удар" => new PoisonStrike(),
            "Пулс" => new Pulse(),
            "Пробиващ удар" => new PiercingStrike(),
            "Потискане" => new Suppress(),
            "Завършващ удар" => new FinalStrike(),
            "Напрежение" => new Exertion(),
            "Стъпка" => new Step(),
            _ => null
        };
    }
}
