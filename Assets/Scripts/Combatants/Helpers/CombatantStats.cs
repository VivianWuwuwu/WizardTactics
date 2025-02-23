using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatantStats
{
    public CappedInt ap;
    public CappedInt health;

    // TODO - Unused
    public int movement;
    public int range;
    public int level;
    public int power;
    public int defense;

    // Giga unused
    public int utility;
    public int resilience;

    public CombatantStats() {}
    public CombatantStats(CombatantStats original) {
        ap = new CappedInt(original.ap);
        health = new CappedInt(original.health);
        movement = original.movement;
        range = original.range;
        level = original.level;
        power = original.power;
        defense = original.defense;
        utility = original.utility;
        resilience = original.resilience;
    }

    public CombatantStats Copy()
    {
        return new CombatantStats(this);
    }
}