using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


/*
A "choice" action has a set of parameters that need to be decided for this action to be performed
*/

// Uhh I guess I can have this expose a builder pattern for the choice?
public abstract class ChoiceAction<Params> : Action 
{
    protected Params choice;

    public virtual void Build(Params given) {
        choice = given;
    }

    public abstract bool ValidateParams(Params given);

    public override (bool, string) CanAct() {
        if (choice == null) {
            return (false, "No parameters selected");
        }
        if (!ValidateParams(choice)) {
            return (false, "Invalid params"); // TODO - Define this reasoning better
        }
        return (true, null);
    }
}