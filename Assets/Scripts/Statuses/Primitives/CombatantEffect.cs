using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CombatantStatus))]
public abstract class CombatantEffect : MonoBehaviour
{
    public virtual int Priority => 0;
    public Combatant Parent {get => GetComponent<CombatantStatus>().Parent;}
}

public abstract class CombatantEvent : CombatantEffect {
    public abstract SubscribableIEnumerator Target();
    public abstract IEnumerator Act();
    public void OnEnable() {
        Target().Subscribe(Act, this, Priority);
    }

    public void OnDisable() {
        Target().Unsubscribe(this);
    }
}

public abstract class CombatantMutation<T> : CombatantEffect {
    public abstract MutatableValue<T> Target();
    public abstract T Mutate(T original);
    public void OnEnable() {
        Target().Mutations.Subscribe(Mutate, this, Priority);
    }

    public void OnDisable() {
        Target().Mutations.Unsubscribe(this);
    }
}