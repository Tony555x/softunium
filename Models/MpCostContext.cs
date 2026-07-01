using Harduni.Skills;

namespace Harduni.Models;

public class MpCostContext : EventContext
{
    public Skill Skill { get; }
    public int BaseCost { get; set; }
    public int CostAdd { get; set; }

    public MpCostContext(Skill skill, int baseCost)
    {
        Skill = skill;
        BaseCost = baseCost;
        CostAdd = 0;
    }

    public int GetTotalCost()
    {
        return System.Math.Max(0, BaseCost + CostAdd);
    }
}
