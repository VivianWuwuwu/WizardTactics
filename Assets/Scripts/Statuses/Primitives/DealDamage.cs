using System.Collections;

public class DealDamage : CombatantEvent
{
    public int Amount;
    public override EditableIEnumerator Target() => Parent.OnRefresh;
    public override IEnumerator Act() {
        Parent.Stats().health -= Amount;
        yield return null;
    }
}
