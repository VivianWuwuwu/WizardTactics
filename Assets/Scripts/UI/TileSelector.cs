using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
UI cursor used to pick a tile. We'll have to extend this to be pretty configurable but ok for now!
*/
public class TileSelector : MonoBehaviour
{
    public Board board;
    public Vector2Int? selection;
    private Vector2Int curr {get => board.FindTile(transform.position);}
    public Func<Vector2Int, bool> selectionCriteria;

    private void Awake() {
        selection = null;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition; 
        mousePos.z = 0f; // Ensure Z is zero for 2D games

        Vector3 worldPos = Camera.current.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f; // Keep it on the 2D plane

        transform.position = worldPos;
        DisplaySelection();
        if (Input.GetMouseButtonDown(0) && SelectionIsValid()) {
            selection = curr;
        }
    }

    private void DisplaySelection() {
        // Need to actually draw a square here
    }

    private bool SelectionIsValid() {
        if (selectionCriteria == null) {
            return true;
        }
        return selectionCriteria(curr);
    }

    private void OnDrawGizmosSelected()
    {
        Color validity = SelectionIsValid() ? Color.blue : Color.red;
        board.GizmosDrawTile(curr, validity);
    }
}
/*
We'll make this more sophisticated later. For now just create a picker that selects a tile whatever
*/
