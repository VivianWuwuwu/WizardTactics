using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ICombatant : Actor {
    public CombatantStats Stats();
    public IEnumerator Refresh();
}

[RequireComponent(typeof(GridElement))]
[RequireComponent(typeof(CombatantBehavior))]
public class Combatant : MonoBehaviour, ICombatant
{
    // We can also mutate fields on the combatants
    public MutatableValue<bool> CanMove;

    // All events that statuses can tap into
    [SerializeField]
    public SubscribableIEnumerator OnRefresh;

    [SerializeField]
    public SubscribableIEnumerator OnMove;

    [SerializeField]
    private DefaultStats defaultStats;
    private CombatantStats stats;

    private void SetupSubscriptions() {
        CanMove = new MutatableValue<bool>(this, false);
        OnRefresh = new SubscribableIEnumerator(this, Refresh);
        OnMove = new SubscribableIEnumerator(this);
    }

    public void Awake() 
    {
        SetupSubscriptions();
        if (defaultStats != null) {
            stats = defaultStats.statline.Copy();
        }
    }

    public CombatantStats Stats() => stats;

    public IEnumerator Refresh() {
        // yield return BeforeRefresh.Invoke();
        // TODO -> We use this to adjust cooldowns, etc
        yield return OnRefresh.Invoke();
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

    [ContextMenu("Perform turn")]
    private void TestTurn() {
        StartCoroutine(Act());
    }

    public void OnValidate() {
        SetupSubscriptions();
        if (defaultStats != null) {
            stats = defaultStats.statline.Copy();
        }
    }
}

