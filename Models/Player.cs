using System;
using System.Collections.Generic;
using Harduni.Enemies;
using Harduni.Skills;
using Harduni.Core;
using Harduni.Items;
using Harduni.Relics;


namespace Harduni.Models;

public class Player : Entity
{
    // Level and XP
    public int Level { get; set; }
    public int Xp { get; set; }
    public int MaxXp => (int)(10 * Level * Math.Pow(1.2, Level-1));
    
    // Alignment
    public int Alignment { get; set; } // Positive = Peturium, Negative = Gamenium
    public int Money { get; set; }

    public Inventory Inventory { get; private set; }
    public List<Skill> Skills { get; set; } = new();
    public List<Skill> EquippedSkills { get; set; } = new();
    public List<Relic> Relics { get; set; } = new();
    public List<Relic> EquippedRelics { get; set; } = new();
    public GameState GameState { get; set; }

    public int BaseMaxSkillSlots { get; set; } = 4;
    public int MaxSkillSlots { get; set; }

    public int BaseMaxWeight { get; set; } = 2;
    public int MaxWeight { get; set; }

    public int BaseMaxRelics { get; set; } = 2;
    public int MaxRelics { get; set; }

    public int Tempo { get; set; }
    public int BaseMaxTempo { get; set; } = 5;
    public int MaxTempo { get; set; }
    public List<Skill> TempoSkills { get; set; } = new();
    public List<Skill> EquippedTempoSkills { get; set; } = new();
    public int BaseMaxTempoSkillSlots { get; set; } = 1;
    public int MaxTempoSkillSlots { get; set; }

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
        BaseMaxMp = 10;
        
        Level = 1;
        Xp = 0;
        Alignment = 0;
        Money = 0;

        Inventory = new Inventory();

        InitFullStats();
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

        int oldMaxHp = BaseMaxHp;
        int oldMaxMp = BaseMaxMp;
        int oldAtk = BaseAttack;
        int oldDef = BaseDefence;
        int oldMag = BaseMagic;
        int oldSpd = BaseSpeed;
        int oldLuck = BaseLuck;
        int oldWis = BaseWisdom;

        BaseMaxHp = (int)(20 + L * 5);
        BaseMaxMp = (int)(10 + L * 2);
        BaseAttack = (int)(10 + L * 2);
        BaseDefence = (int)(6 + L * 1.2);
        BaseMagic = (int)(8 + L * 1.6);
        BaseSpeed = (int)(5 + (double)L / 2);
        BaseLuck = (int)(1);
        BaseWisdom = (int)(0);

        RecalcStats();

        int hpGain = BaseMaxHp - oldMaxHp;
        int mpGain = BaseMaxMp - oldMaxMp;
        int atkGain = BaseAttack - oldAtk;
        int defGain = BaseDefence - oldDef;
        int magGain = BaseMagic - oldMag;
        int spdGain = BaseSpeed - oldSpd;
        int luckGain = BaseLuck - oldLuck;
        int wisGain = BaseWisdom - oldWis;

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
        CheckAndAddSkill(5, new Warcry(), messages);
        CheckAndAddSkill(6, new PassiveDamageBonus(), messages);
        CheckAndAddSkill(7, new Filth(), messages);
        CheckAndAddSkill(8, new GuardSkill(), messages);
        CheckAndAddSkill(9, new HitAndRun(), messages);
        CheckAndAddSkill(10, new QuickStrike(), messages);
        CheckAndAddSkill(11, new Concentration(), messages);
        CheckAndAddSkill(12, new IronSkin(), messages);
        CheckAndAddSkill(13, new PoisonStrike(), messages);
        CheckAndAddSkill(14, new Pulse(), messages);
        CheckAndAddSkill(15, new PiercingStrike(), messages);
        CheckAndAddSkill(16, new MagicAffinity(), messages);
        CheckAndAddSkill(17, new VampiricStrike(), messages);

        CheckAndAddTempoSkills(messages);

        return messages;
    }

    private void CheckAndAddSkill(int minLevel, Skill skill, List<string> messages)
    {
        if (Level >= minLevel && !Skills.Exists(s => s.Name == skill.Name))
        {
            Skills.Add(skill);
            
            if (EquippedSkills.Count < MaxSkillSlots)
            {
                EquippedSkills.Add(skill);
                messages.Add($"+++ НАУЧИХТЕ НОВО УМЕНИЕ: {skill.Name} +++");
            }
            else
            {
                messages.Add($"+++ НАУЧИХТЕ НОВО УМЕНИЕ: {skill.Name} +++");
                messages.Add($"(Може да го екипирате от Кордор, тъй като слотовете ви са пълни.)");
            }
            
            RecalcStats();
        }
    }

    public override StatModContext RecalcStats()
    {
        var ctx = base.RecalcStats();
        MaxSkillSlots = BaseMaxSkillSlots + ctx.SkillSlotsAdd;
        MaxWeight = BaseMaxWeight + ctx.MaxWeightAdd;
        MaxRelics = BaseMaxRelics + ctx.MaxRelicsAdd;
        MaxTempo = BaseMaxTempo + ctx.MaxTempoAdd;
        MaxTempoSkillSlots = BaseMaxTempoSkillSlots + ctx.MaxTempoSkillSlotsAdd;
        return ctx;
    }

    public void CheckAndAddTempoSkills(List<string> messages)
    {
        if (GameState != null && GameState.Flags.ContainsKey("tempo_unlocked"))
        {
            CheckAndAddTempoSkill(3, new Combo(), messages);
            CheckAndAddTempoSkill(6, new HolyLight(), messages);
            CheckAndAddTempoSkill(9, new Rot(), messages);
            CheckAndAddTempoSkill(12, new Accel(), messages);
            CheckAndAddTempoSkill(15, new Nigredo(), messages);
            CheckAndAddTempoSkill(18, new Rhythm(), messages);
        }
    }

    private void CheckAndAddTempoSkill(int minLevel, Skill skill, List<string> messages)
    {
        if (Level >= minLevel && !TempoSkills.Exists(s => s.Name == skill.Name))
        {
            TempoSkills.Add(skill);
            
            if (EquippedTempoSkills.Count < MaxTempoSkillSlots)
            {
                EquippedTempoSkills.Add(skill);
                messages.Add($"+++ НАУЧИХТЕ НОВО ТЕМПО УМЕНИЕ: {skill.Name} +++");
            }
            else
            {
                messages.Add($"+++ НАУЧИХТЕ НОВО ТЕМПО УМЕНИЕ: {skill.Name} +++");
                messages.Add($"(Може да го екипирате от Кордор, тъй като слотовете ви за темпо умения са пълни.)");
            }
            
            RecalcStats();
        }
    }

    public override void TriggerEvent(GameEvent ev, EventContext ctx)
    {
        HandlePermanentBonuses(ev, ctx);

        // Propagate events to equipped relics
        for (int i = EquippedRelics.Count - 1; i >= 0; i--)
        {
            EquippedRelics[i].ProcessEvent(this, ev, ctx);
        }
        
        // Trigger all equipped skills before triggering all statuses
        for (int i = EquippedSkills.Count - 1; i >= 0; i--)
        {
            EquippedSkills[i].ProcessEvent(this, ev, ctx);
        }

        // Trigger all equipped tempo skills
        for (int i = EquippedTempoSkills.Count - 1; i >= 0; i--)
        {
            EquippedTempoSkills[i].ProcessEvent(this, ev, ctx);
        }
        
        base.TriggerEvent(ev, ctx);
    }

    private void HandlePermanentBonuses(GameEvent ev, EventContext ctx)
    {
        if (ev == GameEvent.StatAdd && ctx is StatModContext smc)
        {
            if (GameState?.Flags.ContainsKey("ai_shrine_magic_bonus") == true || GameState?.Flags.ContainsKey("ai_temple_magic_bonus") == true)
            {
                smc.MagAdd += 4;
            }
            if (GameState?.Flags.ContainsKey("tiktok_shrine_def_bonus") == true)
            {
                smc.DefAdd += 4;
            }
            if (GameState?.Flags.ContainsKey("andrew_tate_atk_bonus") == true)
            {
                smc.AtkAdd += 4;
            }
            if (GameState?.Flags.ContainsKey("laptop_skill_slot_bonus") == true)
            {
                smc.SkillSlotsAdd += 1;
            }
        }
    }

    public bool CheckLoss(GameEngine engine)
    {
        if (Hp <= 0)
        {
            engine.State.DungeonData.IsEventActive = false;
            engine.ChangeRootPanel(engine.State.World.DeathPanel);
            return true;
        }
        return false;
    }
}
