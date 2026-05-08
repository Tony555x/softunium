using System;
using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Skills;
using Harduni.Core;
using Harduni.Items;

namespace Harduni.Models;

public class Player : Entity
{
    public int MaxMp { get; set; }
    public int Mp { get; set; }
    
    // Level and XP
    public int Level { get; set; }
    public int Xp { get; set; }
    public int MaxXp => (int)(10 * Level * Math.Pow(1.1, Level-1));
    
    // Alignment
    public int Alignment { get; set; } // Positive = Peturium, Negative = Gamenium
    public int Money { get; set; }

    public Inventory Inventory { get; private set; }
    public List<Skill> Skills { get; set; }

    public Player() : base(
        name: "Бойомир Шамтката (БКПто)", 
        battleName: "Шамтка", 
        maxHp: 20,
        attack: 10,
        defence: 6,
        speed: 5,
        magic: 8,
        wisdom: 0,
        luck: 1
    )
    {
        MaxMp = 10;
        Mp = 10;
        
        Level = 1;
        Xp = 0;
        Alignment = 0;
        Money = 0;

        Inventory = new Inventory();
        Inventory.AddItem(new SmallPotion());

        Skills = new List<Skill>();
    }

    public void AddXp(int amount)
    {
        Xp += amount;
    }

    public List<string> ProcessLevelUps()
    {
        var messages = new List<string>();
        int initialLevel = Level;
        
        while (Xp >= MaxXp)
        {
            Xp -= MaxXp;
            Level++;
        }

        if (Level > initialLevel)
        {
            messages.Add($"\n*** ДОСТИГНАХТЕ НИВО {Level}! ***");
            messages.AddRange(RecalculateBaseStats());
        }

        return messages;
    }

    public List<string> RecalculateBaseStats()
    {
        var messages = new List<string>();
        int L = Level - 1;

        int oldMaxHp = MaxHp;
        int oldMaxMp = MaxMp;
        int oldAtk = Attack;
        int oldDef = Defence;
        int oldMag = Magic;
        int oldSpd = Speed;
        int oldLuck = Luck;
        int oldWis = Wisdom;

        MaxHp = (int)(20 + L * 5 + System.Math.Pow(L, 2) * 0.1);
        MaxMp = (int)(10 + L * 2 + System.Math.Pow(L, 2) * 0.02);
        Attack = (int)(10 + L * 2 + System.Math.Pow(L, 2) * 0.08);
        Defence = (int)(6 + L * 1.5 + System.Math.Pow(L, 2) * 0.04);
        Magic = (int)(8 + L * 1.8 + System.Math.Pow(L, 2) * 0.06);
        Speed = (int)(5 + (double)L / 2 + System.Math.Pow(L, 2) * 0.01);
        Luck = (int)(1 + (double)L / 10 + System.Math.Pow(L, 2) / 500);
        Wisdom = (int)(0 + (double)L / 40);

        int hpGain = MaxHp - oldMaxHp;
        int mpGain = MaxMp - oldMaxMp;
        int atkGain = Attack - oldAtk;
        int defGain = Defence - oldDef;
        int magGain = Magic - oldMag;
        int spdGain = Speed - oldSpd;
        int luckGain = Luck - oldLuck;
        int wisGain = Wisdom - oldWis;

        if (hpGain > 0 || mpGain > 0 || atkGain > 0 || defGain > 0)
        {
            messages.Add($"+{hpGain} Макс. Живот, +{mpGain} Макс. Айрян, +{atkGain} Атака, +{defGain} Защита");
        }

        var otherStatsList = new List<string>();
        if (magGain > 0) otherStatsList.Add($"+{magGain} Магия");
        if (spdGain > 0) otherStatsList.Add($"+{spdGain} Скорост");
        if (luckGain > 0) otherStatsList.Add($"+{luckGain} Късмет");
        if (wisGain > 0) otherStatsList.Add($"+{wisGain} Мъдрост");

        if (otherStatsList.Count > 0)
            messages.Add(string.Join(", ", otherStatsList));

        // Skill checks
        CheckAndAddSkill(2, new HeavyAttack(), messages);
        CheckAndAddSkill(3, new Heal(), messages);
        CheckAndAddSkill(4, new Cleave(), messages);

        return messages;
    }

    private void CheckAndAddSkill(int minLevel, Skill skill, List<string> messages)
    {
        if (Level >= minLevel && !Skills.Exists(s => s.Name == skill.Name))
        {
            Skills.Add(skill);
            messages.Add($"+++ НАУЧИХТЕ НОВО УМЕНИЕ: {skill.Name} +++");
        }
    }


}
