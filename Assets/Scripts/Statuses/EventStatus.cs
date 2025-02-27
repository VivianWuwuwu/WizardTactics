using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventStatus : CombatantStatus
{
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
