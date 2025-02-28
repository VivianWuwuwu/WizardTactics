using System.Collections;
using System;

public class CombinationHelpers {
    // Unrelated helpers
    public static (A, B)? TryCast<A, B>(CombatantStatus statusA, CombatantStatus statusB) {
        if (statusA is A convertedA && statusB is B convertedB) {
            return (convertedA, convertedB);
        }
        if (statusA is B reversedB && statusB is A reversedA) {
            return (reversedA, reversedB);
        }
        return null;
    }

    public static Func<CombatantStatus, CombatantStatus, IEnumerator> MakeCombo<A,B>(Func<A, B, IEnumerator> combine) {
        return (CombatantStatus statusA, CombatantStatus statusB) => {
            var converted = TryCast<A, B>(statusA, statusB);
            if (converted.HasValue) {
                return combine(converted.Value.Item1, converted.Value.Item2);
            }
            return null;
        };
    }
}