using System;

namespace Harduni.Core;

public class Option
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string BaseValue { get; set; }
    public string Info { get; set; }
    public bool IsDisabled { get; set; }
    public Action<GameEngine> OnSelect { get; set; }

    public Option(int id, string text, string info, Action<GameEngine> onSelect, bool isDisabled = false, string? baseValue = null)
    {
        Id = id;
        Text = text;
        BaseValue = baseValue;
        Info = info;
        OnSelect = onSelect;
        IsDisabled = isDisabled;
    }
}
