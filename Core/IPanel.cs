namespace Harduni.Core;

public interface IPanel
{
    void Update(float deltaTime, GameEngine engine);
    void Render(GameEngine engine);
    void ProcessInput(string input, GameEngine engine);
    void OnOpen(GameEngine engine) { }
}
