using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Panels;

public class SaveSlotPanel : IPanel
{
    private bool _isSaving;
    private List<Option> _options = new();
    private string _message = "";
    private int _currentPage = 1;

    public SaveSlotPanel(bool isSaving)
    {
        _isSaving = isSaving;
    }

    public void Update(float deltaTime, GameEngine engine) { }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        string actionName = _isSaving ? "Запис" : "Зареждане";
        string preposition = _isSaving ? "в" : "от";

        for (int i = 1; i <= 9; i++)
        {
            int slotOffset = i;
            int actualSlot = (_currentPage - 1) * 9 + slotOffset;

            var metadata = SaveManager.GetSaveMetadata(actualSlot);
            string statusText;
            if (metadata != null)
            {
                string levelInfo = $" Ниво {metadata.Value.Level}";
                string timeInfo = metadata.Value.TimeSaved ?? "null";
                statusText = $" [{levelInfo}, {timeInfo}]";
            }
            else
            {
                statusText = " [Празен]";
            }

            _options.Add(new Option(slotOffset, $"{actionName} {preposition} Слот {actualSlot}{statusText}", "", (eng) =>
            {
                if (_isSaving)
                {
                    SaveManager.Save(actualSlot, eng);
                    _message = $"Играта е записана в Слот {actualSlot}!";
                }
                else
                {
                    if (SaveManager.SaveExists(actualSlot))
                    {
                        SaveManager.Load(actualSlot, eng);
                        ((StatsPanel)engine.State.World.StatsPanel).CurrentSubPanel = null;
                    }
                    else
                    {
                        _message = $"Слот {actualSlot} е празен!";
                    }
                }
            }));
        }
    }

    public void Render(GameEngine engine)
    {
        BuildOptions(engine);
        string title = _isSaving ? "ЗАПИС НА ИГРА" : "ЗАРЕЖДАНЕ НА ИГРА";
        VConsole.WriteLine($"=== {title} ===");
        if (!string.IsNullOrEmpty(_message))
        {
            VConsole.ForegroundColor = ConsoleColor.Green;
            VConsole.WriteLine($"\n{_message}");
            VConsole.ResetColor();
        }
        
        foreach (var opt in _options)
        {
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
        }

        VConsole.WriteLine($"\n--- Страница {_currentPage} (Слотове {(_currentPage - 1) * 9 + 1} - {_currentPage * 9}) ---");
        VConsole.WriteLine("[ < ] Предишна страница  |  [ > ] Следваща страница");
        VConsole.WriteLine("\n[Въведете номер на слот 1-9, < или > за навигация, или Enter за връщане]");
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        _message = "";
        if (string.IsNullOrWhiteSpace(input))
        {
            ((StatsPanel)engine.State.World.StatsPanel).CurrentSubPanel = null;
            return;
        }

        string trimmed = input.Trim();
        if (trimmed == "<")
        {
            if (_currentPage > 1) _currentPage--;
            return;
        }
        if (trimmed == ">")
        {
            _currentPage++;
            return;
        }

        BuildOptions(engine);
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
