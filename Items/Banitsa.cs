using Harduni.Models;

namespace Harduni.Items;

public class Banitsa : Item
{
    public Banitsa() : base(
        name: "Баница",
        description: "Възстановява 150 Живот.",
        accurateDescription: "Възстановява 150 Живот.",
        usableInBattle: true,
        usableOutsideBattle: true,
        weight: 1
    )
    {
        Value = 80;
    }

    public override string Use(Player player)
    {
        var ctx = player.Heal(150);
        return $"Изядохте баницата и възстановихте {ctx.ActualHealed} Живот.";
    }
}
