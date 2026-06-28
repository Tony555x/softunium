namespace Harduni.Models;

public class MpCostContext : EventContext
{
    public int BaseCost { get; set; }
    public int CostAdd { get; set; }

    public MpCostContext(int baseCost)
    {
        BaseCost = baseCost;
        CostAdd = 0;
    }

    public int GetTotalCost()
    {
        return System.Math.Max(0, BaseCost + CostAdd);
    }
}
