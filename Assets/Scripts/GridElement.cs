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

    private Grid GetGrid() {
        return gameObject.GetComponentInParent<Grid>();
    }

    private void OnDrawGizmosSelected()
    {
        Grid g = GetGrid();

        Vector3 center = g.GetCellCenterWorld(g.WorldToCell(transform.position));

        // Draw the tile this gameobject is centered on
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, g.cellSize);
    }
}