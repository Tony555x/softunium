using Harduni.Core;

namespace Harduni.Locations;

public class TeamworkRoomLocation : Location
{
    public TeamworkRoomLocation(World world) : base(world, "Стая Тиймуърк", "Място за изграждане на екипен дух.")
    {
    }

    public override void InitLinks()
    {
        Options.Add(new Option(1, "Връщане в коридора", "Връща ви обратно в Земите на Кордор.", (eng) => eng.ChangeRootPanel(World.Kordor)));
        Options.Add(new Option(2, "Започване на изследване", "Влизате в подземието.", (eng) => 
        {
            World.TeamworkDungeon.Enter(eng);
        }));
    }
}
