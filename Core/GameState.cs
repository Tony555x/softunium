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
    public string BattleMessage { get; set; } = "Битката започва!";
    public IPanel CurrentSubPanel { get; set; }
    public Skill SelectedSkill { get; set; }
    public int XpGained { get; set; }
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

    public GameState()
    {
        Player = new Player();
        World = new World();
        BattleData = new BattleData();
        DungeonData = new DungeonData();
    }
}
