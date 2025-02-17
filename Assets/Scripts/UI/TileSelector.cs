using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
UI cursor used to pick a tile. We'll have to extend this to be pretty configurable but ok for now!
*/
public class TileSelector : MonoBehaviour
{
    public bool allowSelection;
    private Board board;
    public Vector2Int? selection {get; private set;}
    public Vector2Int position {get => board.FindTile(transform.position);}

    public void Setup(Board board) {
        this.board = board;
        allowSelection = false;
        selection = null;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition; 
        mousePos.z = 0f; // Ensure Z is zero for 2D games

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f; // Keep it on the 2D plane

        transform.position = worldPos;
        DisplaySelection();
        if (Input.GetMouseButtonDown(0) && allowSelection) {
            selection = position;
        }
    }

    private void DisplaySelection() {
        // Need to actually draw a square here
    }

    private void OnDrawGizmosSelected()
    {
        // Color validity = allowSelection? Color.blue : Color.red;
        // board.GizmosDrawTile(position, validity);
    }
}
/*
We'll make this more sophisticated later. For now just create a picker that selects a tile whatever
*/
