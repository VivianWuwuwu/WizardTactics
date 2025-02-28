using System.Collections;

public class MoveDamage : CombatantEvent
{
    public int Amount;
    public override SubscribableIEnumerator Target() => Parent.OnMove;
    public override IEnumerator Act() {
        Parent.Stats().health -= Amount;
        yield return null;
    }
}
