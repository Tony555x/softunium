using Harduni.Core;

namespace Harduni.Locations;

public class IntegrityRoomLocation : Location
{
    public IntegrityRoomLocation(World world) : base(world, "Стая Интегрити", "Място за почтеност и честност.")
    {
    }

    public override void InitLinks()
    {
        Options.Add(new Option(1, "Връщане в коридора", "Връща ви обратно в Земите на Кордор.", (eng) => eng.ChangeRootPanel(World.Kordor)));
        Options.Add(new Option(2, "Започване на изследване", "Влизате в подземието.", (eng) => 
        {
            World.IntegrityDungeon.Enter(eng);
        }));
    }
}
