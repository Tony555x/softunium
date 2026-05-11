using Harduni.Core;

namespace Harduni.Locations;

public class ProgressRoomLocation : Location
{
    public ProgressRoomLocation(World world) : base(world, "Стая Прогрес", "Огромна зала, посветена на вечния напредък.")
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Връщане в коридора", "Връща ви обратно в Земите на Кордор.", (eng) => eng.ChangeRootPanel(World.Kordor)));
        Options.Add(new Option(2, "Започване на изследване", "Влизате в подземието.", (eng) => 
        {
            World.ProgressDungeon.Enter(eng);
        }));
    }
}
