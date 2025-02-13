using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridElement : MonoBehaviour
{
    public void OnValidate() {
        if (GetGrid() == null)
        {
            Debug.LogError($"'{gameObject.name}' requires a parent with a Board component! Fix this before running the game.");
        }
    }

    public Board GetGrid() {
        return gameObject.GetComponentInParent<Board>();
    }

    private void OnDrawGizmosSelected()
    {
        var board = GetGrid();
        board.GizmosDrawTile(board.FindTile(transform.position), Color.blue);
    }
}