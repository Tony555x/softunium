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
        CurrentPanel?.OnOpen(this);

        try 
        {
            VConsole.CursorVisible = false;
        } 
        catch { } // Ignore on unsupported terminals

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        float lastTime = 0f;

        VConsole.Clear();

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

            // Reset cursor to 0,0 instead of VConsole.Clear() to minimize flicker
            VConsole.SetCursorPosition(0, 0); 
            
            CurrentPanel.Render(this);
            
            if (CurrentPanel != null && !CurrentPanel.HideChoicePrompt)
            {
                VConsole.WriteLine("\nИзбор: " + _inputBuffer.ToString() + new string(' ', VConsole.WindowWidth - _inputBuffer.Length - 8)); 
            }
            
            VConsole.Flush();

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
        while (VConsole.KeyAvailable)
        {
            var keyInfo = VConsole.ReadKey(true);
            
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                string input = _inputBuffer.ToString().Trim();
                _inputBuffer.Clear();
                VConsole.Clear(); // Clear safely upon submission to refresh screen geometry
                
                string lowerInput = input.ToLower();
                if (lowerInput == "х" || lowerInput == "хар" || lowerInput == "@")
                {
                    if (CurrentPanel != State.World.StatsPanel)
                    {
                        State.LastLocationPanel = CurrentPanel;
                        PreviousRootPanel = CurrentPanel;
                        CurrentPanel = State.World.StatsPanel;
                        CurrentPanel?.OnOpen(this);
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
        VConsole.Clear();
        CurrentPanel?.OnOpen(this);
    }

    public void ReturnToPreviousRoot()
    {
        if (PreviousRootPanel != null)
        {
            CurrentPanel = PreviousRootPanel;
            PreviousRootPanel = null;
            VConsole.Clear();
            CurrentPanel?.OnOpen(this);
        }
    }

    public void Stop()
    {
        IsRunning = false;
        try 
        {
            VConsole.CursorVisible = true;
        } 
        catch { }
    }
}
