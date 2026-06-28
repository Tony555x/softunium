using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Events;

public class DoorOpeningAnimationPanel : IPanel
{
    private float _time = 0f;
    private readonly Action<GameEngine> _onComplete;

    public bool HideChoicePrompt => true;

    private struct EyeAnimation
    {
        public float RelX; // -1.0 to 1.0 within locker width
        public float RelY; // -1.0 to 1.0 within locker height
        public float MaxRadiusX; // percentage of width
        public float MaxRadiusY; // percentage of height
        public float StartTime;
        public float Duration;

        public EyeAnimation(float relX, float relY, float maxRadiusX, float maxRadiusY, float startTime, float duration)
        {
            RelX = relX;
            RelY = relY;
            MaxRadiusX = maxRadiusX;
            MaxRadiusY = maxRadiusY;
            StartTime = startTime;
            Duration = duration;
        }
    }

    private readonly List<EyeAnimation> _eyes = new()
    {
        new EyeAnimation(0.0f, 0.0f, 0.08f, 0.1f, 8.0f, 2.0f), // First eye
        
        // Next 7 eyes starting from 11.0s with decreasing delays and prefixed locations
        new EyeAnimation(-0.5f, -0.6f, 0.05f, 0.07f, 11.0f, 2.0f),
        new EyeAnimation(0.6f, 0.5f, 0.06f, 0.08f, 11.7f, 2.0f),
        new EyeAnimation(-0.6f, 0.4f, 0.04f, 0.06f, 12.3f, 2.0f),
        new EyeAnimation(0.4f, -0.5f, 0.05f, 0.07f, 12.7f, 2.0f),
        new EyeAnimation(-0.2f, 0.7f, 0.04f, 0.05f, 13.0f, 2.0f),
        new EyeAnimation(0.5f, -0.1f, 0.05f, 0.07f, 13.2f, 2.0f),
        new EyeAnimation(-0.7f, -0.2f, 0.03f, 0.04f, 13.3f, 2.0f)
    };

    public DoorOpeningAnimationPanel(Action<GameEngine> onComplete)
    {
        _onComplete = onComplete;
    }

    public void Update(float deltaTime, GameEngine engine)
    {
        _time += deltaTime;
        if (_time >= 14.5f)
        {
            _onComplete?.Invoke(engine);
        }
    }

    public void OnOpen(GameEngine engine)
    {
        _time = 0f;
        VConsole.Clear();
    }

    public void Render(GameEngine engine)
    {
        int width = 80;
        int height = 24;
        try
        {
            width = VConsole.WindowWidth;
            height = VConsole.WindowHeight;
        }
        catch { }

        int xc = width / 2;
        int yc = height / 2;

        char[,] buffer = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                buffer[x, y] = ' ';
            }
        }

        double doorMaxHalfWidth = width * 0.15;
        double doorHalfHeight = height * 0.35;

        int hingeX = (int)Math.Round(xc - doorMaxHalfWidth);
        int closedRightX = (int)Math.Round(xc + doorMaxHalfWidth);

        int frameTop = (int)Math.Round(yc - doorHalfHeight) - 1;
        int frameBottom = (int)Math.Round(yc + doorHalfHeight) + 1;
        int frameLeft = hingeX - 1;
        int frameRight = closedRightX + 1;

        // Draw locker frame
        for (int y = frameTop; y <= frameBottom; y++)
        {
            for (int x = frameLeft; x <= frameRight; x++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (x == frameLeft || x == frameRight || y == frameTop || y == frameBottom)
                    {
                        buffer[x, y] = '+';
                    }
                }
            }
        }

        // Draw eyes using the same cosine-arc almond shape as BlinkingEyeAnimationPanel
        foreach (var eye in _eyes)
        {
            if (_time < eye.StartTime) continue;

            float elapsed = _time - eye.StartTime;
            float progress = Math.Min(elapsed / eye.Duration, 1.0f);

            double eyeWidth = eye.MaxRadiusX * width * 2.0;
            double maxOpen = eye.MaxRadiusY * height;
            double h = maxOpen * progress;

            if (eyeWidth < 1.0 || h < 0.5) continue;

            double cx = xc + eye.RelX * doorMaxHalfWidth;
            double cy = yc + eye.RelY * doorHalfHeight;
            double halfEye = eyeWidth / 2.0;

            for (int x = frameLeft + 1; x < frameRight; x++)
            {
                if (x < 0 || x >= width) continue;

                double dx = x - cx;
                if (Math.Abs(dx) <= halfEye)
                {
                    double eyeAngle = dx * (Math.PI / eyeWidth);
                    double cosVal = Math.Cos(eyeAngle);
                    
                    double yTop = cy - h * cosVal;
                    double yBottom = cy + h * cosVal;

                    for (int y = frameTop + 1; y < frameBottom; y++)
                    {
                        if (y < 0 || y >= height) continue;

                        // Single char iris at the center
                        if (x == (int)Math.Round(cx) && y == (int)Math.Round(cy) && progress > 0.1f)
                        {
                            buffer[x, y] = 'O';
                        }
                        // Outline of the eye (top & bottom eyelids)
                        else if (Math.Abs(y - yTop) < 0.55 || Math.Abs(y - yBottom) < 0.55)
                        {
                            buffer[x, y] = '@';
                        }
                    }
                }
            }
        }

        double maxAngle = Math.PI * 0.42; // approx 75.6 degrees, door remains plainly visible
        double angle = 0.0;
        if (_time >= 2f && _time < 6f)
        {
            float progress = (_time - 2f) / 4f;
            angle = progress * maxAngle;
        }
        else if (_time >= 6f)
        {
            angle = maxAngle;
        }

        double appW = (doorMaxHalfWidth * 2) * Math.Cos(angle);
        int doorRight = (int)Math.Round(hingeX + appW);

        // Draw opening door (on top of eyes)
        if (appW > 0.5)
        {
            double maxPerspectiveExpansion = 0.4;
            
            for (int x = hingeX; x <= doorRight; x++)
            {
                if (x < 0 || x >= width) continue;

                double t = appW > 0.001 ? (x - hingeX) / appW : 1.0;
                double expansion = 1.0 + Math.Sin(angle) * maxPerspectiveExpansion * t;
                double currentHalfHeight = doorHalfHeight * expansion;
                
                int top = (int)Math.Round(yc - currentHalfHeight);
                int bottom = (int)Math.Round(yc + currentHalfHeight);

                for (int y = top; y <= bottom; y++)
                {
                    if (y >= 0 && y < height)
                    {
                        if (x == hingeX || x == doorRight || y == top || y == bottom)
                        {
                            buffer[x, y] = '#';
                        }
                        else
                        {
                            buffer[x, y] = '|';
                        }
                    }
                }
            }
        }

        VConsole.SetCursorPosition(0, 0);
        
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                if (buffer[x, y] == '@' || buffer[x, y] == 'O')
                {
                    VConsole.ForegroundColor = ConsoleColor.DarkBlue;
                }
                else if (buffer[x, y] == '+' || buffer[x, y] == '#' || buffer[x, y] == '|')
                {
                    VConsole.ForegroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    VConsole.ForegroundColor = ConsoleColor.Gray;
                }
                VConsole.Write(buffer[x, y].ToString());
            }
            VConsole.WriteLine("");
        }
        
        VConsole.ResetColor();
    }

    public void ProcessInput(string input, GameEngine engine)
    {
    }
}
