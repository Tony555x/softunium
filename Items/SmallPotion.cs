using Harduni.Models;

namespace Harduni.Items;

public class SmallPotion : Item
{
    public SmallPotion() : base(
        name: "Малка отвара",
        description: "Възстановява 20 Живот.",
        accurateDescription: "Възстановява 20 Живот.",
        usableInBattle: true,
        usableOutsideBattle: true,
        maxStacks: 5
    )
    {
    }

    public override string Use(Player player)
    {
        var ctx = player.Heal(20);
        return $"Възстановихте {ctx.ActualHealed} Живот.";
    }
}
