using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PathAction : ChoiceAction<Vector2Int>
{
    public abstract bool IsPathable(Vector2Int tile);

    public virtual int? GetRange() {
        return null; // By default range is infinite
    }

    public List<Vector2Int> GetPath() {
        List<Vector2Int> path = Pathfinding.FindPath(GetComponent<GridElement>().GetPosition(), choice, IsPathable);
        return path;
    }

    public override bool IsValid(out string reason) {
        if (!base.IsValid(out reason)) {
            return false;
        }
        List<Vector2Int> path = GetPath();
        if (path == null) {
            reason = "Could not find valid path";
            return false;
        }
        if (GetRange().HasValue && path.Count > GetRange().Value) {
            reason = "Path out of allowed range";
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Board board = GetComponent<GridElement>().GetBoard();
        List<Vector2Int> path = GetPath();
        if (path == null) {
            board.GizmosDrawTile(choice, Color.red);
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
