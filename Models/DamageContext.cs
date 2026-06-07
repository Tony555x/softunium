namespace Harduni.Models;

public class DamageContext : EventContext
{
    public Entity Attacker { get; set; }
    public Entity Target { get; set; }
    public int BaseDamage { get; set; }
    public int DamageTaken { get; set; }
    public bool IsLethal { get; set; }
    public DamageType Type { get; set; }

    public int DamageAdd { get; set; } = 0;
    public float DamageMult { get; set; } = 0.0f;

    public DamageContext(Entity attacker, Entity target, int baseDamage, DamageType type)
    {
        Attacker = attacker;
        Target = target;
        BaseDamage = baseDamage;
        Type = type;
    }

    public float GetMultiplier(float sum)
    {
        if (sum >= 0)
        {
            return 1.0f + sum;
        }
        else
        {
            return 1.0f / (1.0f - sum);
        }
    }
}
