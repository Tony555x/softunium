using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Harduni.Core;

public class GameEngine
{
    public GameState State { get; private set; }
    public IPanel CurrentPanel { get; private set; }
    public IPanel PreviousRootPanel { get; private set; }
    public bool IsRunning { get; private set; }

    private StringBuilder _inputBuffer = new StringBuilder();

    public GameEngine()
    {
        State = new GameState();
    }

    public void Start(IPanel initialPanel)
    {
        CurrentPanel = initialPanel;
        IsRunning = true;
        
        try 
        {
            Console.CursorVisible = false;
        } 
        catch { } // Ignore on unsupported terminals

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        float lastTime = 0f;

        Console.Clear();

        while (IsRunning)
        {
            float currentTime = (float)stopwatch.Elapsed.TotalSeconds;
            float deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            if (CurrentPanel == null)
            {
                Stop();
                break;
            }

            CurrentPanel.Update(deltaTime, this);

            ProcessInputNonBlocking();

            // Reset cursor to 0,0 instead of Console.Clear() to minimize flicker
            Console.SetCursorPosition(0, 0); 
            
            CurrentPanel.Render(this);
            
            Console.WriteLine("\nИзбор: " + _inputBuffer.ToString() + new string(' ', Console.WindowWidth - _inputBuffer.Length - 8)); 
            
            // Limit to ~60 FPS
            int elapsedMs = (int)(stopwatch.Elapsed.TotalMilliseconds - (currentTime * 1000));
            int sleepTime = 16 - elapsedMs;
            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
            }
        }
    }

    private void ProcessInputNonBlocking()
    {
        while (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);
            
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                string input = _inputBuffer.ToString().Trim();
                _inputBuffer.Clear();
                Console.Clear(); // Clear safely upon submission to refresh screen geometry
                
                string lowerInput = input.ToLower();
                if (lowerInput == "х" || lowerInput == "хар")
                {
                    if (CurrentPanel != State.World.StatsPanel)
                    {
                        PreviousRootPanel = CurrentPanel;
                        CurrentPanel = State.World.StatsPanel;
                    }
                }
                else
                {
                    CurrentPanel.ProcessInput(input, this);
                }
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (_inputBuffer.Length > 0)
                {
                    _inputBuffer.Remove(_inputBuffer.Length - 1, 1);
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                _inputBuffer.Append(keyInfo.KeyChar);
            }
        }
    }

    public void ChangeRootPanel(IPanel panel)
    {
        CurrentPanel = panel;
        Console.Clear();
    }

    public void ReturnToPreviousRoot()
    {
        if (PreviousRootPanel != null)
        {
            CurrentPanel = PreviousRootPanel;
            PreviousRootPanel = null;
            Console.Clear();
        }
    }

    public void Stop()
    {
        IsRunning = false;
        try 
        {
            Console.CursorVisible = true;
        } 
        catch { }
    }
}
