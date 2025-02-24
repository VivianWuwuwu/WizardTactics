using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Burn : CombatantStatus, Actor
{
    public override void Combine() {
        foreach (Burn b in Parent.GetComponentsInChildren<Burn>().Where(c => c != this)) {
            Destroy(b.gameObject);
        }
    }

    public override IEnumerator Act() {
        Parent.Stats().health -= 2;
        yield return null;
    }
}
