using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
TODO -> Vivvy got in her own head on this one. Just make this accept a List<Vector2Int> and verify path is solid. No need to have pathfinding
*/
public abstract class PathAbility : ChoiceAbility<List<Vector2Int>>
{
    public abstract bool IsPathable(Vector2Int tile);

    public virtual int? GetRange() {
        return null; // By default range is infinite
    }

    public override bool IsValid(out string reason) {
        if (!base.IsValid(out reason)) {
            return false;
        }
        if (choice == null) {
            reason = "No path given";
            return false;
        }
        List<Vector2Int> fullPath = choice.Prepend(GetComponent<GridElement>().GetPosition()).ToList();
        if (!Pathfinding.IsPath(fullPath)) {
            reason = "Not a contiguous path";
            return false;
        }

        Vector2Int pos = GetComponent<GridElement>().GetPosition();
        if (GetRange().HasValue && choice.Prepend(pos).Count() > GetRange().Value) {
            reason = "Path out of allowed range";
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Board board = GetComponent<GridElement>().GetBoard();
        List<Vector2Int> path = choice;
        if (path == null) {
            return;
        }
        List<Vector2Int> fullPath = choice.Prepend(GetComponent<GridElement>().GetPosition()).ToList();
        if (!Pathfinding.IsPath(fullPath)) {
            return;
        }
        Color color = IsValid() ? Color.green : Color.red;

        var destinations = path.Select(board.GetWorldPosition).ToList();
        destinations.Insert(0, transform.position); // Add our source position
        var pairs = destinations.Zip(destinations.Skip(1), (a, b) => (a, b)).ToList();
        Gizmos.color = color;
        for (int i = 0; i < pairs.Count; i++) {
            var pair = pairs[i];
            if (i == pairs.Count - 1) {
                GizmoExtensions.DrawArrow(pair.a, pair.b);
            } else {
                Gizmos.DrawLine(pair.a, pair.b);
            }
        }
        path.ForEach(t => board.GizmosDrawTile(t, color));
    }
}
