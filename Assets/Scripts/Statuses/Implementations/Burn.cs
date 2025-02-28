using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(Timer))]
public class Burn : CombatantStatusWithBuiltins<Burn>
{
    public override IEnumerator Stack(Burn existing)
    {
        Destroy(existing); // This resets the burn to our fresh burn
        yield return null;
    }
}