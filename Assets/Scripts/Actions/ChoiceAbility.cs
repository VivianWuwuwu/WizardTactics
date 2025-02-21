using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


/*
A "choice" action has a set of parameters that need to be decided for this action to be performed

We'll be able to overload different actions to give more feedback on how they work ig
-> Should the action 
*/

// Uhh I guess I can have this expose a builder pattern for the choice?
public abstract class ChoiceAbility<SelectionType> : BaseAbility 
{
    public SelectionType choice;
    public override bool IsValid(out string reason) {
        reason = null;
        if (choice == null) {
            reason = "No parameters selected";
            return false;
        }
        return true;
    }

    public bool IsValid(SelectionType choice) {
        this.choice = choice;
        return IsValid();
    }
}