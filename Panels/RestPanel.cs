using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Panels;

public class RestPanel : IPanel
{
    private List<Option> _options = new();
    private string _message = "";

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        BuildOptions(engine);
    }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        _options.Add(new Option(1, "Пълно възстановяване", "Възстановява напълно Живота и Айряна.", (eng) => 
        {
            eng.State.Player.Hp = eng.State.Player.MaxHp;
            eng.State.Player.Mp = eng.State.Player.MaxMp;
            _message = "Починахте си добре. Всички показатели са възстановени!";
        }));
        _options.Add(new Option(2, "Умения (Loadout)", "Екипирайте или сменете вашите умения.", (eng) => 
        {
            engine.State.World.Kordor.SetSubPanel(engine.State.World.SkillLoadoutPanel);
            engine.State.World.SkillLoadoutPanel.OnOpen(engine);
        }));
        _options.Add(new Option(3, "Реликви (Loadout)", "Екипирайте или сменете вашите реликви.", (eng) => 
        {
            _message = "Системата за реликви все още не е внедрена.";
        }));
    }

    public void Render(GameEngine engine)
    {
        Console.WriteLine("\n=== ПОЧИВКА В КОРДОР ===");
        if (!string.IsNullOrEmpty(_message)) Console.WriteLine($"\n{_message}");
        Console.WriteLine("\nКакво ще правите?");
        
        foreach (var opt in _options)
        {
            Console.WriteLine($" {opt.Id}. {opt.Text}");
        }
        Console.WriteLine("\n[Натиснете Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        _message = "";
        if (string.IsNullOrWhiteSpace(input))
        {
            engine.State.World.Kordor.SetSubPanel(null);
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
