using System.Collections;

public class DealDamage : EventStatus
{
    public int Amount;
    public override SubscribableIEnumerator Target() => Parent.OnRefresh;
    public override IEnumerator Act() {
        Parent.Stats().health -= Amount;
        yield return null;
    }
}
