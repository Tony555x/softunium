using Harduni.Statuses;

namespace Harduni.Models;

public abstract class Entity
{
    public string Name { get; set; }
    public string BattleName { get; set; }
    public int MaxHp { get; set; }
    public int Hp { get; set; }
    
    // RPG Stats
    public int Attack { get; set; }
    public int Defence { get; set; }
    public int Speed { get; set; }
    public int Magic { get; set; }
    public int Wisdom { get; set; }
    public int Luck { get; set; }
    
    public int MaxMp { get; set; }
    public int Mp { get; set; }
    
    // Base RPG Stats
    public int BaseMaxHp { get; set; }
    public int BaseMaxMp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefence { get; set; }
    public int BaseSpeed { get; set; }
    public int BaseMagic { get; set; }
    public int BaseWisdom { get; set; }
    public int BaseLuck { get; set; }

    public float Energy { get; set; }
    public int InitialEnergyBarSize { get; set; }
    public int EnergyBarSize { get; set; }

    public StatusComponent Status { get; private set; }

    protected Entity(string name, string battleName, int maxHp, int attack, int defence, int speed, int magic, int wisdom, int luck)
    {
        Name = name;
        BattleName = battleName;
        BaseMaxHp = maxHp;
        BaseMaxMp = 0; // Default, can be overridden by specific entities
        BaseAttack = attack;
        BaseDefence = defence;
        BaseSpeed = speed;
        BaseMagic = magic;
        BaseWisdom = wisdom;
        BaseLuck = luck;
        Energy = 0;
        
        Status = new StatusComponent(this);
    }

    public void InitFullStats()
    {
        RecalcStats();
        Hp = MaxHp;
        Mp = MaxMp;
    }

    public virtual void TriggerEvent(GameEvent ev, EventContext ctx)
    {
        Status.TriggerEvent(ev, ctx);
    }

    public virtual StatModContext RecalcStats()
    {
        var ctx = new StatModContext();
        TriggerEvent(GameEvent.StatAdd, ctx);
        TriggerEvent(GameEvent.StatMult, ctx);

        int oldMaxHp = MaxHp;
        int oldMaxMp = MaxMp;

        MaxHp = (int)((BaseMaxHp + ctx.MaxHpAdd) * ctx.GetMultiplier(ctx.MaxHpMult));
        MaxMp = (int)((BaseMaxMp + ctx.MaxMpAdd) * ctx.GetMultiplier(ctx.MaxMpMult));
        Attack = (int)((BaseAttack + ctx.AtkAdd) * ctx.GetMultiplier(ctx.AtkMult));
        Defence = (int)((BaseDefence + ctx.DefAdd) * ctx.GetMultiplier(ctx.DefMult));
        Speed = (int)((BaseSpeed + ctx.SpdAdd) * ctx.GetMultiplier(ctx.SpdMult));
        Magic = (int)((BaseMagic + ctx.MagAdd) * ctx.GetMultiplier(ctx.MagMult));
        Wisdom = (int)((BaseWisdom + ctx.WisAdd) * ctx.GetMultiplier(ctx.WisMult));
        Luck = (int)((BaseLuck + ctx.LuckAdd) * ctx.GetMultiplier(ctx.LuckMult));

        // Clamping logic: if max decreases, clamp current to new max.
        if (MaxHp < oldMaxHp && Hp > MaxHp) Hp = MaxHp;
        if (MaxMp < oldMaxMp && Mp > MaxMp) Mp = MaxMp;

        return ctx;
    }

    public AttackContext PerformAttack(Entity target, int baseDamage)
    {
        var ctx = new AttackContext(this, target, baseDamage);
        TriggerEvent(GameEvent.OnAttack, ctx);
        target.TakeAttack(ctx);
        return ctx;
    }

    public void TakeAttack(AttackContext ctx)
    {
        TriggerEvent(GameEvent.OnAttacked, ctx);
        int incomingDamage = (int)((System.Math.Max(1, ctx.BaseDamage + ctx.DamageAdd - (this.Defence / 2)) ) * ctx.GetMultiplier(ctx.DamageMult));
        TakeDamage(ctx, incomingDamage);
    }

    public void TakeDamage(AttackContext ctx, int damage)
    {
        ctx.DamageTaken = damage;
        this.Hp -= damage;
        if (this.Hp <= 0)
        {
            this.Hp = 0;
            ctx.IsLethal = true;
        }
    }

    public HealContext Heal(int amount)
    {
        var ctx = new HealContext(this, amount, 0); // initial actual is 0 before mod
        TriggerEvent(GameEvent.OnHeal, ctx);
        
        int oldHp = this.Hp;
        this.Hp = System.Math.Min(this.MaxHp, this.Hp + ctx.HealAmount);
        ctx.ActualHealed = this.Hp - oldHp;
        return ctx;
    }

    public HealMpContext HealMp(int amount)
    {
        var ctx = new HealMpContext(this, amount, 0);
        TriggerEvent(GameEvent.OnHealMp, ctx);
        
        int oldMp = this.Mp;
        this.Mp = System.Math.Min(this.MaxMp, this.Mp + ctx.HealAmount);
        ctx.ActualHealed = this.Mp - oldMp;
        return ctx;
    }
}
