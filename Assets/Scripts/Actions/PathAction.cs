using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathAction : ChoiceAction<Vector2Int?>
{
    public abstract bool IsPathable(Vector2Int tile);

    public virtual int? GetRange() {
        return null; // By default range is infinite
    }

    public List<Vector2Int> GetPath() {
        if (!choice.HasValue) {
            return null;
        }
        List<Vector2Int> path = Pathfinding.FindPath(GetComponent<GridElement>().GetPosition(), choice.Value, IsPathable);
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

    // For testing pathing in inspector
    [SerializeField]
    private Vector2Int inspectorChoice;
    private void OnDrawGizmosSelected()
    {
        Board board = GetComponent<GridElement>().GetBoard();
        choice = inspectorChoice;
        List<Vector2Int> path = GetPath();
        if (path == null) {
            board.GizmosDrawTile(inspectorChoice, Color.red);
            return;
        }
        Color color = IsValid() ? Color.green : Color.red;
        path.ForEach(t => board.GizmosDrawTile(t, color));
    }
}
