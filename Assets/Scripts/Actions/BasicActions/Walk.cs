using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Walk : PathAction
{
    public override int? GetRange() {
        return 2; // TODO: Get this from the combatant instead
    }

    public override bool IsPathable(Vector2Int tile)
    {
        Board board = GetComponent<GridElement>().GetBoard();
        return !board.GetTile(tile).Occupied();
    }

    protected override IEnumerator PerformAction()
    {
        var element = GetComponent<GridElement>();
        var board = element.GetBoard();
        var destinations = GetPath().Select(board.GetWorldPosition).ToList();
        foreach (Vector3 dest in destinations) {
            gameObject.transform.position = dest;
            yield return null;
            // TODO -> Add smoothing between each move
        }
    }
}
