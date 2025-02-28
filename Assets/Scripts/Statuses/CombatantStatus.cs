using System.Collections;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CombatantStatus : MonoBehaviour
{
    public Combatant Parent {get => transform.parent?.GetComponent<Combatant>();}
    public void OnValidate() {
        enabled = (Parent != null);
        foreach (CombatantEffect effect in gameObject.GetComponents<CombatantEffect>()) {
            effect.enabled = enabled;
        }
    }

    public virtual IEnumerator Combine(CombatantStatus existing) {
        var combos = StatusCombos.GetCombinations();
        var combination = combos.Select(recipe => recipe(this, existing)).NotNull();
        return combination.FirstOrDefault(); // By default return null if we can't find a valid combination (most cases)
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}

// Built-in that we use for any combatant with 
public abstract class CombatantStatusWithBuiltins<T> : CombatantStatus where T : CombatantStatusWithBuiltins<T> {
    public virtual IEnumerator Stack(T existing) {
        // By default just prevent stacking
        Destroy();
        yield return null;
    }

    public override IEnumerator Combine(CombatantStatus existing) {
        if (existing is T stack) {
            return Stack(stack);
        }
        return base.Combine(existing);
    }
}