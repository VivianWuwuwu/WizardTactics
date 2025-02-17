using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CombatantBehavior : MonoBehaviour
{
    public abstract Task<BaseAction> Decide();
    public abstract Task Populate(BaseAction a);
}
