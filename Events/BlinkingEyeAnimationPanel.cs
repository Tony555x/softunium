using System;
using Harduni.Core;

namespace Harduni.Events;

public class BlinkingEyeAnimationPanel : IPanel
{
    private float _time = 0f;
    private readonly Action<GameEngine> _onComplete;

    public bool HideChoicePrompt => true;

    public BlinkingEyeAnimationPanel(Action<GameEngine> onComplete)
    {
        _onComplete = onComplete;
    }

    public void Update(float deltaTime, GameEngine engine)
    {
        _time += deltaTime;
        if (_time >= 1.0f) // 1 seconds of animation (about 1 blink)
        {
            _onComplete?.Invoke(engine);
        }
    }

    public void OnOpen(GameEngine engine)
    {
        _time = 0f;
        Console.Clear();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                Console.Beep(180, 150);
                System.Threading.Thread.Sleep(50);
                Console.Beep(900, 100);
                System.Threading.Thread.Sleep(50);
                Console.Beep(120, 200);
                System.Threading.Thread.Sleep(50);
                Console.Beep(1300, 120);
            }
            catch { }
        });
    }

    public void Render(GameEngine engine)
    {
        int width = 80;
        int height = 24;
        try
        {
            width = Console.WindowWidth;
            height = Console.WindowHeight;
        }
        catch { }

        // Center coordinates
        int xc = width / 2;
        int yc = height / 2;

        // Eye width
        double eyeWidth = width * 0.5;
        // Max eye opening height
        double maxOpen = height * 0.25;

        // Opening height factor using absolute sine of time
        double openingFactor = Math.Abs(Math.Sin(_time * Math.PI * 1.0)); // 1 blink per second
        double h = maxOpen * openingFactor;

        char[,] buffer = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                buffer[x, y] = ' ';
            }
        }

        double halfEye = eyeWidth / 2.0;

        for (int x = 0; x < width; x++)
        {
            double dx = x - xc;
            if (Math.Abs(dx) <= halfEye)
            {
                double angle = dx * (Math.PI / eyeWidth);
                double cosVal = Math.Cos(angle);
                
                double yTop = yc - h * cosVal;
                double yBottom = yc + h * cosVal;

                for (int y = 0; y < height; y++)
                {
                    double dy = y - yc;
                    
                    // Inside the eye opening
                    if (y >= yTop && y <= yBottom)
                    {
                        // Check if pupil (circle)
                        // Account for character aspect ratio
                        double dist = Math.Sqrt((dx / 2.0) * (dx / 2.0) + dy * dy);
                        if (dist <= 3.0)
                        {
                            buffer[x, y] = '@';
                        }
                        else
                        {
                            buffer[x, y] = '.';
                        }
                    }

                    // Arcs outline
                    if (Math.Abs(y - yTop) < 0.55 || Math.Abs(y - yBottom) < 0.55)
                    {
                        buffer[x, y] = '#';
                    }
                }
            }
        }

        // Output buffer in Red color
        Console.ForegroundColor = ConsoleColor.Red;
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                sb.Append(buffer[x, y]);
            }
            sb.AppendLine();
        }
        Console.SetCursorPosition(0, 0);
        Console.Write(sb.ToString());
        Console.ResetColor();
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        // Ignore inputs during animation
    }
}
