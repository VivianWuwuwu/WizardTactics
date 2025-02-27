using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatantStatus : MonoBehaviour
{
    public Combatant Parent {get => transform.parent?.GetComponent<Combatant>();}
    public void OnValidate() {
        /*
        if (Parent == null)
        {
            Debug.LogError($"'{gameObject.name}' requires a parent Combatant! Fix this before running the game.");
        }
        */
    }
}
// First idea for prefab sorting is to just do it by name. This is ok for stacking but otherwise AWFUL.