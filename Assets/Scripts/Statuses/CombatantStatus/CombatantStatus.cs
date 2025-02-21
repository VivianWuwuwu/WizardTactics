using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
public abstract class CombatantStatus : MonoBehaviour, Actor
{
    public Combatant parent {get => GetComponent<Combatant>();}
    public virtual void Combine() {
        return;
    }

    public void Awake() {
        Combine();
    }

    public abstract IEnumerator Act();
}

public static class CombatantStatusHelpers {
}

/*
Define a "Combine" for statuses. Trigger it on Awake.
Soul stacks just combine their numbers
*/