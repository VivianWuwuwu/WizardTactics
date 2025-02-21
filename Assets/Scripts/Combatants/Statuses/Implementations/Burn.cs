using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ignited : PlayerStatus
{
    public override IEnumerator Tic() {
        GetComponent<Combatant>(); // <- Play an animation then -> apply some damage to combatant
        yield return null;
    }
}
