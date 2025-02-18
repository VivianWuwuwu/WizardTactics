using UnityEngine;

public class DamageInfo
{
    public int BaseDamage { get; }
    public Combatant Source { get; }

    public DamageInfo(int baseDamage, Combatant source)
    {
        BaseDamage = baseDamage;
        Source = source;
    }
}
// ^ TODO -> Append to this

public interface AttackModifier {
    public DamageInfo ModifyAttack(DamageInfo given);
} 

public interface DefenseModifier {
    public DamageInfo ModifyDefense(DamageInfo given);
} 