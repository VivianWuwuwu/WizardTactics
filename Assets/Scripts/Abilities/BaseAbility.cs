using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
ANY action a player makes (Moving, Casting a spell, Teleporting, etc) is considered an Action
*/
[RequireComponent(typeof(GridElement))]
public abstract class BaseAbility : MonoBehaviour
{
    public EditableIEnumerator Action; // <- This is compiled by the class

    [ContextMenu("Compile")]
    public abstract bool Compile(out string debug);


    [ContextMenu("Act")]
    private void ActInInspector() {
        IEnumerator action = Act();
        if (action == null) {
            return;
        }
        while (action.MoveNext()); // evalute the full action
    }

    public IEnumerator Act() {
        string reason;
        if (!IsValid(out reason)) {
            Debug.Log($"Cannot act - {reason}");
            return null;
        }
        return PerformAction();
    }

    public abstract bool IsValid(out string reason);
    public bool IsValid() => IsValid(out _); // Overload to use IsValid without debug message
    protected abstract IEnumerator PerformAction();
}
