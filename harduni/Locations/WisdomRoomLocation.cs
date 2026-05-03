using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Locations;

public class WisdomRoomLocation : Location
{
    public WisdomRoomLocation(World world) : base(world, "Стая Уйздом", "Тайнствена стая, изпълнена с древно познание.")
    {
    }

    public override void InitLinks()
    {
        Options.Add(new Option(1, "Връщане в коридора", "Връща ви обратно в Земите на Кордор.", (eng) => eng.ChangeRootPanel(World.Kordor)));
        Options.Add(new Option(2, "Започване на изследване", "Влизате в подземието.", (eng) => 
        {
            World.WisdomDungeon.Initialize(eng);
            eng.ChangeRootPanel(World.WisdomDungeon);
        }));
    }
}
