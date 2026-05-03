using System;

namespace Harduni.Core;

public class Option
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Info { get; set; }
    public Action<GameEngine> OnSelect { get; set; }

    public Option(int id, string text, string info, Action<GameEngine> onSelect)
    {
        Id = id;
        Text = text;
        Info = info;
        OnSelect = onSelect;
    }
}
