using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Panels;

public class SaveSlotPanel : IPanel
{
    private bool _isSaving;
    private List<Option> _options = new();

    public SaveSlotPanel(bool isSaving)
    {
        _isSaving = isSaving;
    }

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        string actionName = _isSaving ? "Запис" : "Зареждане";

        for (int i = 1; i <= 3; i++)
        {
            int slot = i;
            string status = SaveManager.SaveExists(slot) ? " [Съществува]" : " [Празен]";
            _options.Add(new Option(slot, $"{actionName} в Слот {slot}{status}", "", (eng) =>
            {
                if (_isSaving)
                {
                    SaveManager.Save(slot, eng);
                    Console.WriteLine($"\nИграта е записана в Слот {slot}!");
                }
                else
                {
                    if (SaveManager.SaveExists(slot))
                    {
                        SaveManager.Load(slot, eng);
                        Console.WriteLine($"\nИграта е заредена от Слот {slot}!");
                        ((StatsPanel)engine.State.World.StatsPanel).CurrentSubPanel = null;
                    }
                    else
                    {
                        Console.WriteLine($"\nСлот {slot} е празен!");
                    }
                }
                Console.WriteLine("Натиснете Enter за продължаване...");
            }));
        }
    }

    public void Render(GameEngine engine)
    {
        BuildOptions(engine);
        string title = _isSaving ? "ЗАПИС НА ИГРА" : "ЗАРЕЖДАНЕ НА ИГРА";
        Console.WriteLine($"=== {title} ===");
        
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
        }
        Console.WriteLine("\n[Въведете номер на слот или Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            ((StatsPanel)engine.State.World.StatsPanel).CurrentSubPanel = null;
            return;
        }

        BuildOptions(engine);
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
