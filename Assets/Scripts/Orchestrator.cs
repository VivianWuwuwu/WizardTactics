using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Orchestrator is responsible for managing a single turn
*/
[RequireComponent(typeof(ElementLocator))]
public class Orchestrator : MonoBehaviour
{
    /*
    TODO?

    [SerializeField]
    private List<Combatant> turnQueue;

    private void PopulateTurn() {
        // ... TODO
    }

    public async void ExecuteTurn() {
        PopulateTurn();
        while (turnQueue.Count > 0) {
            var current = turnQueue[0];
            turnQueue.RemoveAt(0);

            // await current.Act();
        }
    }
    */
}
