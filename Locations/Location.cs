using System;
using System.Collections.Generic;
using Harduni.Core;

namespace Harduni.Locations;

public abstract class Location : IPanel
{
    protected World World { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Option> Options { get; set; }
    public string LocationMessage { get; set; } = "";

    public Location(World world, string name, string description)
    {
        World = world;
        Name = name;
        Description = description;
        Options = new List<Option>();
    }

    public virtual void Update(float deltaTime, GameEngine engine) { }

    public virtual void InitLinks()
    {
        // Override in derived classes to setup links and options
    }

    public virtual void Render(GameEngine engine)
    {
        Console.WriteLine($"=== {Name} ===");
        Console.WriteLine(Description);
        
        if (!string.IsNullOrEmpty(LocationMessage))
        {
            Console.WriteLine($"\n[ {LocationMessage} ]");
        }
        
        Console.WriteLine("\nВъзможни действия:");
        foreach (var option in Options)
        {
            Console.WriteLine($" {option.Id}. {option.Text}");
        }
    }

    public virtual void ProcessInput(string input, GameEngine engine)
    {
        LocationMessage = ""; 
        if (!InputHandler.Handle(input, Options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
