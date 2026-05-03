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
    
    public float Energy { get; set; }

    protected Entity(string name, string battleName, int maxHp, int attack, int defence, int speed, int magic, int wisdom, int luck)
    {
        Name = name;
        BattleName = battleName;
        MaxHp = maxHp;
        Hp = maxHp;
        Attack = attack;
        Defence = defence;
        Speed = speed;
        Magic = magic;
        Wisdom = wisdom;
        Luck = luck;
        Energy = 0;
    }

    public AttackContext PerformAttack(Entity target, int baseDamage)
    {
        var ctx = new AttackContext(this, target, baseDamage);
        target.TakeAttack(ctx);
        return ctx;
    }

    public void TakeAttack(AttackContext ctx)
    {
        int incomingDamage = System.Math.Max(1, ctx.BaseDamage - (this.Defence / 2));
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
}
