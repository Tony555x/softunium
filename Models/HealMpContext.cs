namespace Harduni.Models;

public class HealMpContext : EventContext
{
    public Entity Target { get; set; }
    public int HealAmount { get; set; }
    public int ActualHealed { get; set; }

    public HealMpContext(Entity target, int healAmount, int actualHealed)
    {
        Target = target;
        HealAmount = healAmount;
        ActualHealed = actualHealed;
    }
}
