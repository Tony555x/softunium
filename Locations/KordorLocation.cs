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
        SetSubPanel(null); // Ensure no subpanel is active when returning to Kordor

        Options.Add(new Option(1, "Почивка", "Възстановяване и управление на екипировката.", (eng) => 
        {
            SetSubPanel(World.RestPanel);
            World.RestPanel.OnOpen(eng);
        }));

        Options.Add(new Option(2, "Към Стая Уйздом", "Път, който води към тайнствената Стая Уйздом.", (eng) => eng.ChangeRootPanel(World.WisdomRoom)));
        
        int offset = 2;

        if (engine.State.Flags.ContainsKey("WisdomRoomsUnlocked"))
        {
            Options.Add(new Option(3, "Към Стая Прогрес", "Път към Стаята на Прогреса.", (eng) => eng.ChangeRootPanel(World.ProgressRoom)));
            Options.Add(new Option(4, "Към Стая Тиймуърк", "Път към Стаята за екипна работа.", (eng) => eng.ChangeRootPanel(World.TeamworkRoom)));
            Options.Add(new Option(5, "Към Стая Интегрити", "Път към Стаята за почтеност.", (eng) => eng.ChangeRootPanel(World.IntegrityRoom)));
            offset = 5;
        }

        Options.Add(new Option(offset + 1, "Изход", "Спира играта.", (eng) => eng.Stop()));
    }

    public override void Update(float deltaTime, GameEngine engine)
    {
        // Keep empty to prevent memory leak
    }
}
