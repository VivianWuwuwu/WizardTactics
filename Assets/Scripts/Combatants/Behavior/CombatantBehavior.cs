using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CombatantBehavior : MonoBehaviour
{
    public abstract Task<Action> Decide();

    // Generic method for compile-time resolution
    public Task Populate<T>(T action) where T : Action
    {
        return Populate(action); // Calls the most specific overload
    }

    public abstract Task Populate(Action a);
}
