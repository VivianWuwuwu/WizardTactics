using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DamageCalculator
{
    // Helper methods for all damage calculations
    public static int GetDamage(DamageInfo damage, Combatant target) {
        var finalDamage = ApplyModifiers(damage, target);

        return -1; // TODO
    }

    private static DamageInfo ApplyModifiers(DamageInfo damage, Combatant target) {
        AttackModifier[] attackModifiers = damage.Source.GetComponents<AttackModifier>();
        DefenseModifier[] defenseModifiers = target.GetComponents<DefenseModifier>();

        foreach (AttackModifier modifier in attackModifiers) {
            damage = modifier.ModifyAttack(damage);
        }
        foreach (DefenseModifier modifier in defenseModifiers) {
            damage = modifier.ModifyDefense(damage);
        }
        return damage;
    }
}
