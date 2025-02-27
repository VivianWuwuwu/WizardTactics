using System.Collections;

public class Timer : CombatantEffect
{
    public int Duration; // how many turns this lasts for
    public override int GetPriority() => -1;
    public override SubscribableIEnumerator Target() => Parent.OnRefresh;

    public override IEnumerator Act() {
        yield return null;
        Duration--;
        if (Duration == 0) {
            Destroy(gameObject);
        }
    }
}