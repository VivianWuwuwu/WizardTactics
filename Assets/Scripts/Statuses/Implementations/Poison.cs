using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(DealDamage))]
public class Poison : CombatantStatusWithBuiltins<Poison> {
    public override IEnumerator Stack(Poison existing)
    {
        GetComponent<DealDamage>().Amount += existing.GetComponent<DealDamage>().Amount;
        Destroy(existing);
        yield return null;
    }
}
