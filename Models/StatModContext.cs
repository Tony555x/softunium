namespace Harduni.Models;

public class StatModContext : EventContext
{
    public int MaxHpAdd { get; set; } = 0;
    public float MaxHpMult { get; set; } = 0.0f;

    public int MaxMpAdd { get; set; } = 0;
    public float MaxMpMult { get; set; } = 0.0f;

    public int AtkAdd { get; set; } = 0;
    public float AtkMult { get; set; } = 0.0f;

    public int DefAdd { get; set; } = 0;
    public float DefMult { get; set; } = 0.0f;

    public int SpdAdd { get; set; } = 0;
    public float SpdMult { get; set; } = 0.0f;

    public int MagAdd { get; set; } = 0;
    public float MagMult { get; set; } = 0.0f;

    public int WisAdd { get; set; } = 0;
    public float WisMult { get; set; } = 0.0f;

    public int LuckAdd { get; set; } = 0;
    public float LuckMult { get; set; } = 0.0f;

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
