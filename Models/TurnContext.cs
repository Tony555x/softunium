using Harduni.Core;

namespace Harduni.Models;

public class TurnContext : EventContext
{
    public GameEngine Engine { get; }

    public TurnContext(GameEngine engine)
    {
        Engine = engine;
    }
}
