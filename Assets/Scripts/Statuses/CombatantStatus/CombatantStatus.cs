using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
public abstract class CombatantStatus : MonoBehaviour
{
    public Combatant Parent {get => transform.parent?.GetComponent<Combatant>();}
    public void OnValidate() {
        if (Parent == null)
        {
            Debug.LogError($"'{gameObject.name}' requires a parent Combatant! Fix this before running the game.");
        }
    }

    public virtual void Combine() {
        return;
    }

    public void Awake() {
        Combine();
    }

    public abstract IEnumerator Act();
}