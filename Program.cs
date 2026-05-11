using System;
using System.Text;
using Harduni.Core;

namespace Harduni;

class Program
{
    public static GameEngine Engine { get; private set; }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Engine = new GameEngine();



        // Start game at Kordor
        Engine.Start(Engine.State.World.Kordor);
    }
}
