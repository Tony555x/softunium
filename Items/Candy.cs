using Harduni.Models;
using Harduni.Statuses;

namespace Harduni.Items;

public class Candy : Item
{
    public Candy() : base(
        name: "Бонбони",
        description: "Дава +15 регенерация за тази битка.",
        accurateDescription: "Добавя статус PersistentRegenStatus с 15 сила за 1 битка.",
        usableInBattle: true,
        usableOutsideBattle: false,
        weight: 1
    )
    {
        Value = 150;
    }

    public override string Use(Player player)
    {
        player.Status.AddStatus(new PersistentRegenStatus(1, 15));
        return "Изядохте бонбоните и усещате прилив на енергия! (+15 Регенерация)";
    }
}
