using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
public abstract class PlayerStatus : MonoBehaviour
{
    public abstract IEnumerator Tic(); // idk ?
}

/*
Define a "Combine" for statuses. Trigger it on Awake.
Soul stacks just combine their numbers
*/