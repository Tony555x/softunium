using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Skills;
using Harduni.Locations;

namespace Harduni.Core;

public class BattleData
{
    public List<Enemy> Enemies { get; set; } = new List<Enemy>();
    public bool IsFinished { get; set; } = false;
    public bool IsPlayerTurn { get; set; } = false;
    public float PlayerDeathTimer { get; set; } = 0f;
    public List<string> BattleLog { get; set; } = new List<string> { "Битката започва!" };
    public void Log(string message) => BattleLog.Add(message);
    public void ClearLog() => BattleLog.Clear();
    public IPanel CurrentSubPanel { get; set; }
    public Skill SelectedSkill { get; set; }
    public int XpGained { get; set; }
    public int MoneyGained { get; set; }
    public float LootMultiplier { get; set; } = 1.0f;
    public IPanel SourcePanel { get; set; }
}

public class DungeonData
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public int CurrentRoomIndex { get; set; } = 0;
    public bool IsEventActive { get; set; } = false;
}

public class GameState
{
    public Player Player { get; set; }
    public World World { get; set; }
    public BattleData BattleData { get; set; }
    public DungeonData DungeonData { get; set; }
    public Dictionary<string, string> Flags { get; set; } = new();
    public IPanel? LastLocationPanel { get; set; }
    public GameEngine Engine { get; set; }

    public GameState()
    {
        Player = new Player();
        Player.GameState = this;
        World = new World();
        BattleData = new BattleData();
        DungeonData = new DungeonData();
    }
}
