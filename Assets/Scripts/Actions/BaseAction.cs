using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
ANY action a player makes (Moving, Casting a spell, Teleporting, etc) is considered an Action
*/
[RequireComponent(typeof(GridElement))]
public abstract class BaseAction : MonoBehaviour
{
    [ContextMenu("Act")]
    private void ActInInspector() {
        string reason;
        if (!IsValid(out reason)) {
            Debug.Log($"Cannot act - Reason: {reason}");
            return;
        }
        IEnumerator frames = PerformAction();
        while (frames.MoveNext()); // evalute the full action
    }

    public IEnumerator Act() {
        string reason;
        if (!IsValid(out reason)) {
            Debug.Log($"Cannot act - Reason: {reason}");
            return null;
        }
        return PerformAction();
    }

    public abstract bool IsValid(out string reason);
    public bool IsValid() => IsValid(out _); // Overload to use IsValid without debug message
    protected abstract IEnumerator PerformAction();
}
