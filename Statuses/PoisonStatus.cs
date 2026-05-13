using Harduni.Models;
using System;

namespace Harduni.Statuses;

public class PoisonStatus : Status
{
    public int Stacks { get; set; }

    public PoisonStatus(int stacks)
    {
        Stacks = stacks;
    }

    public override void OnStack(Status newStatus)
    {
        if (newStatus is PoisonStatus p)
        {
            Stacks += p.Stacks;
        }
    }

    public override string GetDisplayString()
    {
        return $"[Отрова {Stacks}]";
    }

    public override string GetDescription()
    {
        return $"Нанася {Stacks} щети в края на хода. Губи 1/3 от силата си всеки ход.";
    }

    public override void ProcessEvent(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.EndTurn && ctx is TurnContext tCtx)
        {
            var battleData = tCtx.Engine.State.BattleData;
            
            // Take damage
            int dmg = Stacks;
            Owner.Hp -= dmg;
            if (Owner.Hp <= 0) Owner.Hp = 0; // Prevent death, or let it kill? Poison usually kills or leaves at 1 HP, let's let it kill.

            // Print to battle log
            battleData.Log($"{Owner.Name} поема {dmg} щети от отрова.");

            // Reduce stacks by 1/3, rounded down, min 1
            int reduction = Math.Max(Stacks / 3, 1);
            Stacks -= reduction;

            if (Stacks <= 0)
            {
                Destroy();
            }
        }
    }
}
