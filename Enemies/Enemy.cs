using Harduni.Core;
using Harduni.Models;

namespace Harduni.Enemies;

public abstract class Enemy : Entity
{
    public int XpReward { get; set; }
    public int MoneyReward { get; set; }

    protected Enemy(string name, int maxHp, int attack, int defence, int speed, int magic, int wisdom, int luck, int xpReward, int moneyReward) 
        : base(name, name, maxHp, attack, defence, speed, magic, wisdom, luck)
    {
        XpReward = xpReward;
        MoneyReward = moneyReward;
    }

    public abstract void TakeAction(GameEngine engine);
}
