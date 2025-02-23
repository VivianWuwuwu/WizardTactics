using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Burn : CombatantStatus, Actor
{
    public override void Combine() {
        foreach (Burn b in parent.GetComponents<Burn>().Where(c => c != this)) {
            Destroy(b);
            Debug.Log("Delete other instances");
        }
    }

    public override IEnumerator Act() {
        // Damage parent?
        yield return null;
    }
}
