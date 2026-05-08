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
        Options.Add(new Option(2, "Почивка", "Възстановява напълно Живота и Айряна ви.", (eng) => 
        {
            eng.State.Player.Hp = eng.State.Player.MaxHp;
            eng.State.Player.Mp = eng.State.Player.MaxMp;
            this.LocationMessage = "Починахте си добре. Всички показатели са възстановени!";
        }));
        Options.Add(new Option(3, "Изход", "Спира играта.", (eng) => eng.Stop()));
    }
}
