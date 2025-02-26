using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Burn : CombatantStatus
{
    public void Awake() {
        // Parent.BeforeRefresh += BurnPlayer;
    }

    public IEnumerator BurnPlayer() {
        Parent.Stats().health -= 2;
        yield return null;
    }
}
