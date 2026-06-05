using Harduni.Models;

namespace Harduni.Items;

public class Water : Item
{
    public Water() : base(
        name: "Вода",
        description: "Възстановява 30 Айрян.",
        accurateDescription: "Възстановява 30 Айрян.",
        usableInBattle: true,
        usableOutsideBattle: true,
        weight: 1
    )
    {
        Value = 120;
    }

    public override string Use(Player player)
    {
        var ctx = player.HealMp(30);
        return $"Изпихте водата и възстановихте {ctx.ActualHealed} Айрян.";
    }
}
