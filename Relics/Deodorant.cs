using Harduni.Models;

namespace Harduni.Relics;

public class Deodorant : Relic
{
    public Deodorant() : base(
        name: "Дезодорант",
        description: "Мирише на евтино, но поне не мирише на пот.",
        accurateDescription: "В момента няма ефект."
    )
    {
    }
}
