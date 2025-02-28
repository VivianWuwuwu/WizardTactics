using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Duration))]
[RequireComponent(typeof(Stun))]
public class Entangle : CombatantStatusWithBuiltins<Entangle> {
    private int amount = 1;
    public int StackAmount {
        get => amount;
        set {
            GetComponent<Stun>().enabled = StackAmount >= 3;
        }
    }

    public override IEnumerator Stack(Entangle existing)
    {
        amount += existing.amount;
        Destroy(existing);
        yield return null;
    }
}
