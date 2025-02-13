using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CombatantBehavior : MonoBehaviour
{
    public abstract Task<Action> Decide();
    public abstract Task Populate(Action a);
}
