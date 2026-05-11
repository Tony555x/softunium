using Harduni.Core;

namespace Harduni.Locations;

public class KordorLocation : Location
{
    public KordorLocation(World world) : base(world, "Земите на Кордор", "Намирате се в мрачните и пусти земи на Кордор.")
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Към Стая Уйздом", "Път, който води към тайнствената Стая Уйздом.", (eng) => eng.ChangeRootPanel(World.WisdomRoom)));
        
        int offset = 1;

        if (engine.State.Flags.ContainsKey("WisdomRoomsUnlocked"))
        {
            Options.Add(new Option(2, "Към Стая Прогрес", "Път към Стаята на Прогреса.", (eng) => eng.ChangeRootPanel(World.ProgressRoom)));
            Options.Add(new Option(3, "Към Стая Тиймуърк", "Път към Стаята за екипна работа.", (eng) => eng.ChangeRootPanel(World.TeamworkRoom)));
            Options.Add(new Option(4, "Към Стая Интегрити", "Път към Стаята за почтеност.", (eng) => eng.ChangeRootPanel(World.IntegrityRoom)));
            offset = 4;
        }

        Options.Add(new Option(offset + 1, "Почивка", "Възстановява напълно Живота и Айряна ви.", (eng) => 
        {
            eng.State.Player.Hp = eng.State.Player.MaxHp;
            eng.State.Player.Mp = eng.State.Player.MaxMp;
            this.LocationMessage = "Починахте си добре. Всички показатели са възстановени!";
        }));
        Options.Add(new Option(offset + 2, "Изход", "Спира играта.", (eng) => eng.Stop()));
    }

    public override void Update(float deltaTime, GameEngine engine)
    {
        // Keep empty to prevent memory leak
    }
}
