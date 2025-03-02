using System.Collections;

public class Stun : CombatantMutation<bool>
{
    public override EditableValue<bool> Target() => Parent.CanMove;
    public override bool Mutate(bool original) {
        return false;
    }
}
