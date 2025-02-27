using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CombatantStatus))]
public abstract class CombatantEffect : MonoBehaviour
{
    public Combatant Parent {get => GetComponent<CombatantStatus>().Parent;}

    public abstract SubscribableIEnumerator Target();
    public abstract IEnumerator Act();
    public virtual int GetPriority() => 0;

    public void OnEnable() {
        Target().Subscribe(Act, this, GetPriority());
    }

    public void OnDisable() {
        Target().Unsubscribe(Act);
    }
}
// We can use this to compose like 99% of primitives