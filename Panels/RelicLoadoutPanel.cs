using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;
using Harduni.Items;
using Harduni.Relics;


namespace Harduni.Panels;

public class RelicLoadoutPanel : IPanel
{
    private List<Option> _options = new();
    private string _message = "";
    private string _slotStatus = "";
    private int _page = 0;
    private const int _pageSize = 20;

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        BuildOptions(engine);
    }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var p = engine.State.Player;

        _slotStatus = $"Слотове за реликви: {p.EquippedRelics.Count}/{p.MaxRelics}";

        int startIndex = _page * _pageSize;
        int endIndex = Math.Min(startIndex + _pageSize, p.Relics.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            var relic = p.Relics[i];
            bool isEquipped = p.EquippedRelics.Contains(relic);
            string status = isEquipped ? "[ЕКИПИРАНО]" : "[НЕЕКИПИРАНО]";

            string info = relic.AccurateDescription;

            _options.Add(new Option(i + 1, $"{status} {relic.Name}: {relic.Description}", info, (eng) => 
            {
                if (isEquipped)
                {
                    p.EquippedRelics.Remove(relic);
                    _message = $"Премахнахте {relic.Name}.";
                }
                else
                {
                    if (p.EquippedRelics.Count >= p.MaxRelics)
                    {
                        _message = "Нямате свободни слотове за реликви!";
                    }
                    else
                    {
                        p.EquippedRelics.Add(relic);
                        _message = $"Екипирахте {relic.Name}.";
                    }
                }
                p.RecalcStats();
                BuildOptions(eng);
            }, false, relic.Name));
        }

        if (_page > 0)
        {
            _options.Add(new Option(-1, "<< Предишна страница", "", (eng) => 
            {
                _page--;
                BuildOptions(eng);
            }));
        }

        if (endIndex < p.Relics.Count)
        {
            _options.Add(new Option(-2, "Следваща страница >>", "", (eng) => 
            {
                _page++;
                BuildOptions(eng);
            }));
        }

        _options.Add(new Option(0, "Изчисти", "Маха всички екипирани реликви.", (eng) => 
        {
            p.EquippedRelics.Clear();
            p.RecalcStats();
            _message = "Всички реликви са премахнати.";
            BuildOptions(eng);
        }));
    }

    public void Render(GameEngine engine)
    {
        VConsole.WriteLine("\n=== ЕКИПИРОВКА НА РЕЛИКВИ ===");
        VConsole.WriteLine(_slotStatus);
        if (!string.IsNullOrEmpty(_message)) 
        {
            VConsole.WriteLine();
            VConsole.WriteLine(_message);
        }
        VConsole.WriteLine($"\nСтраница: {_page + 1}");
        VConsole.WriteLine("\nИзберете реликва за екипиране/премахване:");
        
        foreach (var opt in _options)
        {
            if (opt.Id == -1 || opt.Id == -2 || opt.Id == 0)
            {
                VConsole.WriteLine($" { (opt.Id <= 0 ? (opt.Id == 0 ? "0" : (opt.Id == -1 ? "<" : ">")) : opt.Id.ToString()) }. {opt.Text}");
                continue;
            }

            if (opt.Text.StartsWith("[ЕКИПИРАНО]"))
            {
                VConsole.ForegroundColor = ConsoleColor.Green;
                VConsole.Write($" {opt.Id}. [ЕКИПИРАНО]");
                VConsole.ResetColor();
                VConsole.WriteLine($" {opt.Text.Substring(11)}");
            }
            else
            {
                VConsole.ForegroundColor = ConsoleColor.Red;
                VConsole.Write($" {opt.Id}. [НЕЕКИПИРАНО]");
                VConsole.ResetColor();
                VConsole.WriteLine($" {opt.Text.Substring(14)}");
            }
        }
        VConsole.WriteLine("\n[< - Предишна | > - Следваща | 0 - Изчисти]");
        VConsole.WriteLine("[Натиснете Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        _message = "";
        if (string.IsNullOrWhiteSpace(input))
        {
            engine.State.World.Kordor.SetSubPanel(engine.State.World.RestPanel);
            engine.State.World.RestPanel.OnOpen(engine);
            return;
        }

        string normalizedInput = input.Trim().ToUpper();
        if (normalizedInput == "<")
        {
            var opt = _options.Find(o => o.Id == -1);
            opt?.OnSelect?.Invoke(engine);
            return;
        }
        if (normalizedInput == ">")
        {
            var opt = _options.Find(o => o.Id == -2);
            opt?.OnSelect?.Invoke(engine);
            return;
        }

        if (!InputHandler.Handle(input, _options, out Option selectedOption, info => _message = info))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
