using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Burn : CombatantStatus
{
    public override void Combine() {
        IEnumerable<Burn> others = parent.GetComponents<Burn>().Where(c => c != this);
        foreach (Burn b in others) {
            Destroy(b);
            Debug.Log("Deleting stack");
        }
        return;
    }

    public override IEnumerator Act() {
        // Damage parent?
        yield return null;
    }
}
