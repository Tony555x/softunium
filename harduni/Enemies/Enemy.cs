using Harduni.Core;
using Harduni.Models;

namespace Harduni.Enemies;

public abstract class Enemy : Entity
{
    public int XpReward { get; set; }

    protected Enemy(string name, int maxHp, int attack, int defence, int speed, int magic, int wisdom, int luck, int xpReward) 
        : base(name, name, maxHp, attack, defence, speed, magic, wisdom, luck)
    {
        XpReward = xpReward;
    }

    public abstract void TakeAction(GameEngine engine);
}
