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
public class Combatant : MonoBehaviour, Actor
{
    [SerializeField]
    private DefaultStats defaultStats;
    public CombatantStats stats;

    public void Awake() {
        if (defaultStats != null) {
            stats = defaultStats.statline.Copy();
        }
    }

    [ContextMenu("Perform turn")]
    private void TestTurn() {
        StartCoroutine(Act());
    }

    public IEnumerator Refresh() {
        // TODO -> We use this to adjust cooldowns, etc
        yield return null;
    }

    public IEnumerator Act() {
        yield return Refresh();
        Debug.Log("Deciding");
        Task<BaseAbility> decision = ((dynamic)GetComponent<CombatantBehavior>()).Decide();
        yield return new WaitUntil(() => decision.IsCompleted);
        Debug.Log("Decided...");
        BaseAbility action = decision.Result;
        yield return action.Act();
    }

    public void OnValidate() {
        if (defaultStats != null) {
            stats = defaultStats.statline.Copy();
        }
    }
}

