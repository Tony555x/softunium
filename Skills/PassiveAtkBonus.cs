using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;

namespace Harduni.Skills;

public class PassiveAtkBonus : Skill
{
    public PassiveAtkBonus() : base("Сила на духа", "Пасивно: +20% Атака.", "Увеличава общата Атака с 20%.", TargetType.Self, 0, false, false) 
    {
        Keywords.Add("percent");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        return ""; // Passive
    }

    public override void ProcessEvent(Entity owner, GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.StatMult && ctx is StatModContext smc)
        {
            smc.AtkMult += 0.2f;
        }
    }
}
