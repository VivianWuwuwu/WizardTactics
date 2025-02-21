using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

/*
Probably will be used by statuses to modify player stats. Just an idea that is reliant on underlying Combatant methods changing
*/
public class CombatantOverride
{
    public static CombatantOverrideBuilder FromCombatant(Combatant baseCombatant)
    {
        return new CombatantOverrideBuilder(baseCombatant);
    }

    public static CombatantOverrideBuilder FromOverride(CombatantOverride baseOverride)
    {
        return new CombatantOverrideBuilder(baseOverride);
    }

    public Combatant Wrapped {get; private set;}
    public Func<DamageInfo, DamageInfo> ApplyAttack {get; private set;}

    // Empty constructor
    private CombatantOverride(Combatant baseCombatant)
    {
        this.Wrapped = baseCombatant;
        this.ApplyAttack = (damage) => Wrapped.ApplyAttack(damage);
    }

    // Builder yaaa
    private CombatantOverride(CombatantOverride baseOverride) : this(baseOverride.Wrapped)
    {
        this.ApplyAttack = baseOverride.ApplyAttack;
    }

    // Expose a builder as well!
    public class CombatantOverrideBuilder
    {
        private CombatantOverride combatantOverride;

        public CombatantOverrideBuilder(Combatant baseCombatant)
        {
            combatantOverride = new CombatantOverride(baseCombatant);
        }

        public CombatantOverrideBuilder(CombatantOverride baseOverride)
        {
            combatantOverride = new CombatantOverride(baseOverride);
        }

        public CombatantOverrideBuilder OverrideAttack(Func<DamageInfo, DamageInfo> attackFunc)
        {
            combatantOverride.ApplyAttack = attackFunc;
            return this;
        }

        public CombatantOverride Build() => combatantOverride;
    }
}