using System.Collections;

public class Timer : CombatantEvent
{
    public override int Priority => -1;
    public int Duration; // how many turns this lasts for
    public override EditableIEnumerator Target() => Parent.OnRefresh;

    public override IEnumerator Act() {
        yield return null;
        Duration--;
        if (Duration == 0) {
            GetComponent<CombatantStatus>().Destroy();
        }
    }
}