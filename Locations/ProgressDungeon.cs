using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Enemies;
using Harduni.Events;

namespace Harduni.Locations;

public class ProgressDungeon : Dungeon
{
    private static readonly Random _rand = new();

    public ProgressDungeon(World world) : base(world, "Подземие на Прогрес", "Изключително дълго и трудно подземие.")
    {
    }

    public override void OnOpen(GameEngine engine)
    {
        base.OnOpen(engine);
        Options.Add(new Option(1, "Напред", "Продължава към следващата стая или битка.", (engine) => GoForward(engine)));
        Options.Add(new Option(2, "Бягство", "Връща ви в предходната локация.", (engine) => PerformEscape(engine)));
    }

    public override void Enter(GameEngine engine)
    {
        var data = engine.State.DungeonData;
        data.CurrentRoomIndex = 0;
        data.IsEventActive = false;

        var rooms = new List<Room>();

        for (int i = 0; i < 21; i++)
        {
            if (i == 11)
            {
                // Shop event
                if (engine.State.Flags.ContainsKey("shop_unlocked"))
                {
                    rooms.Add(new Room(0, null, new BreakEvent()));
                }
                else
                {
                    rooms.Add(new Room(0, null, new ShopUnlockEvent()));
                }
                continue;
            }

            if (i == 20)
            {
                // Boss room placeholder
                rooms.Add(new Room(3, new List<Enemy> { new OligofrenBoss(), new StuckProgrammer() }));
                continue;
            }

            var enemies = new List<Enemy>();
            float mult=1.0f;
            
            if (i <= 2) // 0-2
            {
                enemies.Add(i%2 == 0 ? new Programmer() : new SmellyProgrammer());
            }
            else if(i==3){
                enemies.Add(new WeakProgrammer());
                enemies.Add(new WeakProgrammer());
                enemies.Add(new WeakProgrammer());
                enemies.Add(new WeakProgrammer());
                enemies.Add(new WeakProgrammer());
                enemies.Add(new WeakProgrammer());
            }
            else if(i==4){
                enemies.Add(new Programmer());
                enemies.Add(new Beta());
                enemies.Add(new WeakProgrammer());
            }
            else if (i==5) // 5-6
            {
                mult=1.5f;
                enemies.Add(new Programmer());
                enemies.Add(new SmellyProgrammer());
            }
            else if (i==6) // 5-6
            {
                mult=1.5f;
                enemies.Add(new Programmer());
                enemies.Add(new Programmer());
            }
            else if (i==7) // 5-6
            {
                mult=1.5f;
                enemies.Add(new SmellyProgrammer());
                enemies.Add(new SmellyProgrammer());
            }
            else if (i==8){
                mult=2f;
                enemies.Add(new Programmer());
                enemies.Add(new Programmer());
                enemies.Add(new Programmer());
            }
            else if (i==9){
                mult=2f;
                enemies.Add(new SmellyProgrammer());
                enemies.Add(new SmellyProgrammer());
                enemies.Add(new SmellyProgrammer());
            }
            else if (i==10){
                mult=2f;
                enemies.Add(new Programmer());
                enemies.Add(new SmellyProgrammer());
                enemies.Add(new SmellyProgrammer());
            }
            else if (i<=13){//12-13
                enemies.Add(new StuckProgrammer());
            }
            else if (i <= 15) // 13-14
            {
                mult=1.5f;
                enemies.Add(new StuckProgrammer());
                enemies.Add(i%2 == 0 ? new Programmer() : new SmellyProgrammer());
            }
            else if (i <= 17)// 15-16
            {
                mult=2f;
                enemies.Add(new StuckProgrammer());
                enemies.Add(new Programmer());
                enemies.Add(new SmellyProgrammer());
            }
            else if(i==18)// 18
            {
                mult=2f;
                enemies.Add(new StuckProgrammer());
                enemies.Add(new StuckProgrammer());
            }else{
                mult=2.5f;
                enemies.Add(new StuckProgrammer());
                enemies.Add(new StuckProgrammer());
                enemies.Add(new Programmer());
                enemies.Add(new SmellyProgrammer());
            }

            var room = new Room(1, enemies);
            
            room.LootMultiplier = mult;
            
            rooms.Add(room);
        }

        data.Rooms = rooms;
        engine.ChangeRootPanel(this);
    }

    protected override void Escape(GameEngine engine)
    {
        engine.ChangeRootPanel(World.ProgressRoom);
    }
}
