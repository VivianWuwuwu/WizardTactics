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
public class Combatant : Actor
{
    public int speed; // uhhh yeah sure

    [ContextMenu("Perform turn")]
    private void TestTurn() {
        StartCoroutine(Act());
    }

    public IEnumerator Refresh() {
        // TODO -> We use this to adjust cooldowns, etc
        yield return null;
    }

    public override IEnumerator Act() {
        yield return Refresh();
        Debug.Log("Deciding");
        Task<BaseAction> decision = ((dynamic)GetComponent<CombatantBehavior>()).Decide();
        yield return new WaitUntil(() => decision.IsCompleted);
        Debug.Log("Decided...");
        BaseAction action = decision.Result;
        yield return action.Act();
    }

    public DamageInfo ApplyAttack(DamageInfo baseDamage) {
        return baseDamage;
    }
}
