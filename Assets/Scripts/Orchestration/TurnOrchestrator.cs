using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Attach this to a board, use to to control when each player goes
[RequireComponent(typeof(Board))]
public class TurnOrchestrator : MonoBehaviour
{
    private ISet<Actor> Actors() {
        return transform.Cast<Transform>().Select(child => child.GetComponent<Actor>()).Where(e => e != null).ToHashSet();
    }

    // Any time something is ADDED to the queue, it's added to right before the end (curr)
    public List<Actor> ordering;
}
