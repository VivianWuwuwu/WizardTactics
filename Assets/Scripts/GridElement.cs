using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridElement : MonoBehaviour
{
    public void OnValidate() {
        if (GetBoard() == null)
        {
            Debug.LogError($"'{gameObject.name}' requires a parent with a Board component! Fix this before running the game.");
        }
    }

    public Board GetBoard() {
        return gameObject.GetComponentInParent<Board>();
    }

    public Vector2Int GetPosition() {
        return GetBoard().FindTile(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        var board = GetBoard();
        board.GizmosDrawTile(board.FindTile(transform.position), Color.blue);
    }
}