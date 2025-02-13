using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
Moves ->
Are these scriptable objects? Idk??
*/

[RequireComponent(typeof(GridElement))]
[RequireComponent(typeof(CombatantBehavior))]
public class Combatant : MonoBehaviour
{
    public int speed; // uhhh yeah sure

    [ContextMenu("Perform turn")]
    private void TestTurn() {
        StartCoroutine(Act());
    }

    public IEnumerator Act() {
        Debug.Log("Deciding");
        var decision = GetComponent<CombatantBehavior>().Decide();
        yield return new WaitUntil(() => decision.IsCompleted);
        Debug.Log("Decided...");
        Action action = decision.Result;
        // Finally, trigger that action?
    }
}
