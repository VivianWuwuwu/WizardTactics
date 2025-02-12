using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
ANY action a player makes (Moving, Casting a spell, Teleporting, etc) is considered an Action
*/
public abstract class Action : MonoBehaviour
{
    public void Act() {
        var (canAct, why) = CanAct();
        if (!canAct) {
            Debug.Log($"Cannot act - Reason: {why}");
            return;
        }
        PerformAction();
    }

    public abstract (bool, string) CanAct();
    protected abstract void PerformAction();
}
