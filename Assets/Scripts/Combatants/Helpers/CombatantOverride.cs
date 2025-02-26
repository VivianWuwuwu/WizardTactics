using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

/*
Probably will be used by statuses to modify player stats. Just an idea that is reliant on underlying Combatant methods changing
*/

public class CombatantOverride : ICombatant
{
    protected ICombatant source;

    public CombatantOverride(ICombatant source) {
        this.source = source;
    }

    public virtual IEnumerator Act() => source.Act();
    public virtual IEnumerator Refresh() => source.Refresh();
    public virtual CombatantStats Stats() => source.Stats();
}