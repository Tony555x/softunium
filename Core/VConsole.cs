using System;
using System.Text;

public struct VConsoleCell
{
    public char Char;
    public ConsoleColor Foreground;
    public ConsoleColor Background;
}

public static class VConsole
{
    private static VConsoleCell[] _buffer;
    public static int BufferWidth { get; private set; }
    public static int BufferHeight { get; private set; }

    public static ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;
    public static ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
    public static int CursorLeft { get; set; } = 0;
    public static int CursorTop { get; set; } = 0;
    public static bool CursorVisible 
    { 
        get
        {
            try { return System.Console.CursorVisible; }
            catch { return false; }
        }
        set
        {
            try { System.Console.CursorVisible = value; }
            catch { }
        }
    }
    public static int WindowWidth => System.Console.WindowWidth;
    public static int WindowHeight => System.Console.WindowHeight;

    public static Encoding OutputEncoding 
    { 
        get => System.Console.OutputEncoding; 
        set => System.Console.OutputEncoding = value; 
    }
    public static Encoding InputEncoding 
    { 
        get => System.Console.InputEncoding; 
        set => System.Console.InputEncoding = value; 
    }
    public static bool KeyAvailable => System.Console.KeyAvailable;

    static VConsole()
    {
        InitializeBuffer(System.Console.WindowWidth, System.Console.WindowHeight);
    }

    private static void InitializeBuffer(int width, int height)
    {
        BufferWidth = width;
        BufferHeight = height;
        _buffer = new VConsoleCell[width * height];
        Clear();
    }

    private static void CheckResize()
    {
        int w = System.Console.WindowWidth;
        int h = System.Console.WindowHeight;
        if (BufferWidth != w || BufferHeight != h)
        {
            // We need to keep the old buffer content? 
            // Usually resizing clears or jumbles the console anyway.
            // But let's just reinitialize.
            InitializeBuffer(w, h);
        }
    }

    public static void Write(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        CheckResize();

        foreach (char c in text)
        {
            if (c == '\n')
            {
                CursorLeft = 0;
                CursorTop++;
            }
            else if (c == '\r')
            {
                CursorLeft = 0;
            }
            else
            {
                if (CursorLeft >= BufferWidth)
                {
                    CursorLeft = 0;
                    CursorTop++;
                }

                if (CursorTop < BufferHeight)
                {
                    int index = CursorTop * BufferWidth + CursorLeft;
                    _buffer[index] = new VConsoleCell 
                    { 
                        Char = c, 
                        Foreground = ForegroundColor, 
                        Background = BackgroundColor 
                    };
                }
                CursorLeft++;
            }
        }
    }

    public static void WriteLine(string text)
    {
        Write(text + "\n");
    }

    public static void WriteLine()
    {
        Write("\n");
    }

    public static void SetCursorPosition(int left, int top)
    {
        CheckResize();
        CursorLeft = Math.Max(0, Math.Min(left, BufferWidth - 1));
        CursorTop = Math.Max(0, Math.Min(top, BufferHeight - 1));
    }

    public static void Clear()
    {
        CheckResize();
        for (int i = 0; i < _buffer.Length; i++)
        {
            _buffer[i] = new VConsoleCell 
            { 
                Char = ' ', 
                Foreground = ForegroundColor, 
                Background = BackgroundColor 
            };
        }
        CursorLeft = 0;
        CursorTop = 0;
    }

    public static void ResetColor()
    {
        ForegroundColor = ConsoleColor.Gray;
        BackgroundColor = ConsoleColor.Black;
    }

    public static string ReadLine()
    {
        Flush();
        return System.Console.ReadLine();
    }

    public static ConsoleKeyInfo ReadKey()
    {
        Flush();
        return System.Console.ReadKey();
    }

    public static ConsoleKeyInfo ReadKey(bool intercept)
    {
        Flush();
        return System.Console.ReadKey(intercept);
    }

    public static void Flush()
    {
        CheckResize();

        // Actually draw the buffer to the screen
        System.Console.SetCursorPosition(0, 0);

        ConsoleColor currentFg = System.Console.ForegroundColor;
        ConsoleColor currentBg = System.Console.BackgroundColor;
        
        StringBuilder chunk = new StringBuilder();
        ConsoleColor chunkFg = _buffer[0].Foreground;
        ConsoleColor chunkBg = _buffer[0].Background;

        for (int y = 0; y < BufferHeight; y++)
        {
            for (int x = 0; x < BufferWidth; x++)
            {
                VConsoleCell cell = _buffer[y * BufferWidth + x];

                if (cell.Foreground != chunkFg || cell.Background != chunkBg)
                {
                    // Flush chunk
                    if (chunk.Length > 0)
                    {
                        System.Console.ForegroundColor = chunkFg;
                        System.Console.BackgroundColor = chunkBg;
                        System.Console.Write(chunk.ToString());
                        chunk.Clear();
                    }
                    chunkFg = cell.Foreground;
                    chunkBg = cell.Background;
                }

                chunk.Append(cell.Char);
            }
            
            // Do not append newline here since we are just writing character by character
            // Writing at the end of the line will automatically wrap if the console is configured to do so,
            // or we might need to be careful not to write the very last character of the bottom-right corner 
            // to avoid scrolling.
            if (y == BufferHeight - 1)
            {
                // Remove the last character to avoid scrolling the console down by 1 line
                if (chunk.Length > 0)
                {
                    chunk.Length--;
                }
            }
        }

        if (chunk.Length > 0)
        {
            System.Console.ForegroundColor = chunkFg;
            System.Console.BackgroundColor = chunkBg;
            System.Console.Write(chunk.ToString());
        }

        // Restore original system colors
        System.Console.ForegroundColor = currentFg;
        System.Console.BackgroundColor = currentBg;
        
        // Restore logical cursor position for any subsequent direct console input
        try
        {
            System.Console.SetCursorPosition(CursorLeft, CursorTop);
        }
        catch { }

        System.Console.Out.Flush();
    }
}
