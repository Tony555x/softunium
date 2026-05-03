using Harduni.Core;

namespace Harduni.Locations;

public class KordorLocation : Location
{
    public KordorLocation(World world) : base(world, "Земите на Кордор", "Намирате се в мрачните и пусти земи на Кордор.")
    {
    }

    public override void InitLinks()
    {
        Options.Add(new Option(1, "Към Стая Уйздом", "Път, който води към тайнствената Стая Уйздом.", (eng) => eng.ChangeRootPanel(World.WisdomRoom)));
        Options.Add(new Option(2, "Изход", "Спира играта.", (eng) => eng.Stop()));
    }
}
