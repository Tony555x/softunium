namespace Harduni.Models;

public class AttackContext : EventContext
{
    public Entity Attacker { get; set; }
    public Entity Target { get; set; }
    public int BaseDamage { get; set; }
    public int DamageTaken { get; set; }
    public bool IsLethal { get; set; }

    public AttackContext(Entity attacker, Entity target, int baseDamage)
    {
        Attacker = attacker;
        Target = target;
        BaseDamage = baseDamage;
    }
}
