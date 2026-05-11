using System.Collections.Generic;
using Harduni.Core;
using Harduni.Events;

namespace Harduni.Locations;

public class TeamworkDungeon : Dungeon
{
    public TeamworkDungeon(World world) : base(world, "Подземие на Тиймуърк", "Място за изграждане на екипен дух.")
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Напред", "Продължава към следващата стая.", (engine) => GoForward(engine)));
        Options.Add(new Option(2, "Бягство", "Връща ви в предходната локация.", (engine) => PerformEscape(engine)));
    }

    public override void Enter(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>();
        rooms.Add(new Room(0, null, new BreakEvent())); // Placeholder

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.TeamworkRoom);
    }
}
