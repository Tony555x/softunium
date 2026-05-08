namespace Harduni.Models;

public class HealContext
{
    public Entity Target { get; set; }
    public int HealAmount { get; set; }
    public int ActualHealed { get; set; }

    public HealContext(Entity target, int healAmount, int actualHealed)
    {
        Target = target;
        HealAmount = healAmount;
        ActualHealed = actualHealed;
    }
}
