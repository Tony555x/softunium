using System;
using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Skills;

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

    public List<Item> Inventory { get; set; }
    public List<Skill> Skills { get; set; }

    public Player() : base(
        "Бойомир Шамтката (БКПто)", 
        "Шамтка", 
        20,  // MaxHp
        10,  // Attack
        6,   // Defence
        5,   // Speed
        8,   // Magic
        0,   // Wisdom
        1    // Luck
    )
    {
        MaxMp = 50;
        Mp = 50;
        
        Level = 1;
        Xp = 0;
        Alignment = 0;

        Inventory = new List<Item>();
        Inventory.Add(new Item("Малка отвара", "Възстановява 20 Живот.", (p) => p.Hp = System.Math.Min(p.Hp + 20, p.MaxHp)));

        Skills = new List<Skill>();
    }

    public void AddXp(int amount)
    {
        Xp += amount;
    }

    public List<string> ProcessLevelUps()
    {
        var messages = new List<string>();
        
        while (Xp >= MaxXp)
        {
            Xp -= MaxXp;
            Level++;
            
            int L = Level - 1;
            
            int newMaxHp = (int)(20 + L * 5 + System.Math.Pow(L, 2) * 0.1);
            int newMaxMp = (int)(10 + L * 2 + System.Math.Pow(L, 2) * 0.02);
            int newAtk = (int)(10 + L * 2 + System.Math.Pow(L, 2) * 0.08);
            int newDef = (int)(6 + L * 1.5 + System.Math.Pow(L, 2) * 0.04);
            int newMag = (int)(8 + L * 1.8 + System.Math.Pow(L, 2) * 0.06);
            int newSpd = (int)(5 + (double)L / 2 + System.Math.Pow(L, 2) * 0.01);
            int newLuck = (int)(1 + (double)L / 10 + System.Math.Pow(L, 2) / 500);
            int newWis = (int)(0 + (double)L / 40);

            int hpGain = newMaxHp - MaxHp;
            int mpGain = newMaxMp - MaxMp;
            int atkGain = newAtk - Attack;
            int defGain = newDef - Defence;
            int magGain = newMag - Magic;
            int spdGain = newSpd - Speed;
            int luckGain = newLuck - Luck;
            int wisGain = newWis - Wisdom;

            MaxHp = newMaxHp;
            MaxMp = newMaxMp;
            Attack = newAtk;
            Defence = newDef;
            Magic = newMag;
            Speed = newSpd;
            Luck = newLuck;
            Wisdom = newWis;

            messages.Add($"\n*** ДОСТИГНАХТЕ НИВО {Level}! ***");
            messages.Add($"+{hpGain} Макс. Живот (Общо: {MaxHp})");
            messages.Add($"+{mpGain} Макс. Айрян (Общо: {MaxMp})");
            messages.Add($"+{atkGain} Атака (Общо: {Attack})");
            messages.Add($"+{defGain} Защита (Общо: {Defence})");
            if(spdGain > 0) messages.Add($"+{spdGain} Скорост (Общо: {Speed})");
            if (magGain > 0) messages.Add($"+{magGain} Магия (Общо: {Magic})");
            if (wisGain > 0) messages.Add($"+{wisGain} Мъдрост (Общо: {Wisdom})");
            if (luckGain > 0) messages.Add($"+{luckGain} Късмет (Общо: {Luck})");
        }

        return messages;
    }
}
