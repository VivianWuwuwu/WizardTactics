using System;
using System.Collections;
using System.Collections.Generic;
using static CombinationHelpers;

public class StatusCombos {
    public static List<Func<CombatantStatus, CombatantStatus, IEnumerator>> GetCombinations() => new List<Func<CombatantStatus, CombatantStatus, IEnumerator>>{
        // INSERT RECIPES HERE
        MakeCombo<Water, Burn>(Water.Extinguish),
        MakeCombo<Water, Electrify>(Electrify.Conduct),
        MakeCombo<Water, Shocked>(Shocked.Conduct),
    };
}