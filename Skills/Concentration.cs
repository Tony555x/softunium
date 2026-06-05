using System.Collections.Generic;
using Harduni.Models;
using Harduni.Enemies;
using Harduni.Statuses;

namespace Harduni.Skills;

public class Concentration : Skill
{
    public override string Name => "Концентрация";
    public override string ShortDescription => "Увеличава магията и лекува.";
    public override string AccurateDescription => "+25% Магия за 5 хода и възстановява (Магия * 0.4) Живот.";
    public override TargetType Target => TargetType.Self;
    public override int MpCost => 4;
    public override bool UsableInBattle => true;
    public override bool UsableOutsideBattle => false;
    public override int BaseCooldown => 1;

    public Concentration()
    {
        Keywords.Add("percent");
    }

    public override string Execute(Player player, List<Enemy> allEnemies, Enemy target)
    {
        player.Status.ApplyStatus(new MagUpStatus(5, 0.25f));
        player.RecalcStats();
        
        int healAmount = (int)(player.Magic * 0.4);
        var ctx = player.Heal(healAmount);
        
        return $"Концентрирахте се (+25% Магия) и възстановихте {ctx.ActualHealed} Живот.";
    }
}
