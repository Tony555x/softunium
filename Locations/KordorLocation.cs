using Harduni.Core;

namespace Harduni.Locations;

public class KordorLocation : Location
{
    public KordorLocation(World world) : base(world, "Земите на Кордор", "Намирате се в мрачните и пусти земи на Кордор.")
    {
    }

    private bool _confirmExit = false;

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        SetSubPanel(null); // Ensure no subpanel is active when returning to Kordor
        _confirmExit = false;
        BuildOptions(engine);
    }

    private void BuildOptions(GameEngine engine)
    {
        Options.Clear();
        if (_confirmExit)
        {
            Description = "ВНИМАНИЕ: Незапазеният прогрес ще бъде загубен! Сигурни ли сте, че искате да излезете?";
            Options.Add(new Option(1, "Да, излез", "Спира играта без запис.", (eng) => eng.Stop()));
            Options.Add(new Option(2, "Не, върни се", "Връщане обратно.", (eng) => 
            {
                _confirmExit = false;
                Description = "Намирате се в мрачните и пусти земи на Кордор.";
                BuildOptions(eng);
            }));
            return;
        }

        int id = 1;
        Options.Add(new Option(id++, "Почивка", "Възстановяване и управление на екипировката.", (eng) => 
        {
            SetSubPanel(World.RestPanel);
            World.RestPanel.OnOpen(eng);
        }));

        if (engine.State.Flags.ContainsKey("shop_unlocked"))
        {
            Options.Add(new Option(id++, "Лафка", "Купете предмети от Стоянов.", (eng) => eng.ChangeRootPanel(World.ShopPanel)));
        }

        Options.Add(new Option(id++, "Към Стая Уйздом", "Път, който води към тайнствената Стая Уйздом.", (eng) => eng.ChangeRootPanel(World.WisdomRoom)));

        if (engine.State.Flags.ContainsKey("WisdomRoomsUnlocked"))
        {
            Options.Add(new Option(id++, "Към Стая Прогрес", "Път към Стаята на Прогреса.", (eng) => eng.ChangeRootPanel(World.ProgressRoom)));
            Options.Add(new Option(id++, "Към Стая Тиймуърк", "Път към Стаята за екипна работа.", (eng) => eng.ChangeRootPanel(World.TeamworkRoom)));
            Options.Add(new Option(id++, "Към Стая Лийдършип", "Път към Стаята за лидерство.", (eng) => eng.ChangeRootPanel(World.LeadershipRoom)));
        }

        Options.Add(new Option(id++, "Изход", "Спира играта.", (eng) => 
        {
            _confirmExit = true;
            BuildOptions(eng);
        }));

        Options.Add(new Option(99, "Дебъг локация (Debug)", "Отидете в тайната дебъг зона.", (eng) => 
        {
            eng.ChangeRootPanel(World.DebugLocation);
        }));
    }

    public override void Update(float deltaTime, GameEngine engine)
    {
        // Keep empty to prevent memory leak
    }
}
