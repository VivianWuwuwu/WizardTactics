using System.Collections;

public class MoveDamage : CombatantStatus
{
    public int Amount;
    public void OnEnable() {
        Parent.OnRefresh.Subscribe(Damage, this);
    }

    public void OnDisable() {
        Parent.OnRefresh.Unsubscribe(Damage);
    }

    public IEnumerator Damage() {
        Parent.Stats().health -= Amount;
        yield return null;
    }
}
