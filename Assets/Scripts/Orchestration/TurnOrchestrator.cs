using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

// Attach this to a board, use to to control when each player goes
[RequireComponent(typeof(Board))]
public class TurnOrchestrator : MonoBehaviour
{
    public int idx {get => idx; set => idx = value % ordering.Count;}
    public List<Actor> ordering; // <- For now just do it like this lol
    public IEnumerator PerformTurn() {
        if (!ordering.Any()) {
            return null;
        }
        Actor curr = ordering.First();
        ordering.RemoveAt(0);
        if (curr == null) {
            return PerformTurn();
        }
        ordering.Add(curr);
        return curr.Act();
    }

    private void Insert(Actor added) {
        // ordering.Add(ordering - 1 );
    }

    // TODO -> We should have this scan for new actors, add em to the queue
    // That automatically handles all tile statuses
}
