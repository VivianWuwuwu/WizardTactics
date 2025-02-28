using System.Collections;

public class Stun : CombatantMutation<bool>
{
    public override SubscribableMutation<bool> Target() => Parent.CanMove;
    public override bool Mutate(bool original) {
        return false;
    }
}
