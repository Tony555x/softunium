using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Locations;

public class WisdomRoomLocation : Location
{
    public WisdomRoomLocation(World world) : base(world, "Стая Уйздом", "Тайнствена стая, изпълнена с древно познание.")
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Връщане в коридора", "Връща ви обратно в Земите на Кордор.", (eng) => eng.ChangeRootPanel(World.Kordor)));
        Options.Add(new Option(2, "Започване на изследване", "Влизате в подземието.", (eng) => 
        {
            World.WisdomDungeon.Enter(eng);
        }));
    }
}
