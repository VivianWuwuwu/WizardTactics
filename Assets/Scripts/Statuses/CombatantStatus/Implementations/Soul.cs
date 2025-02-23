using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Soul : CombatantStatus
{
    public int stack = 1;

    public override void Combine() {
        foreach (Soul b in Parent.GetComponentsInChildren<Soul>().Where(c => c != this)) {
            stack += b.stack;
            Destroy(b.gameObject);
        }
    }

    public override IEnumerator Act() {
        // Damage parent?
        yield return null;
    }
}
