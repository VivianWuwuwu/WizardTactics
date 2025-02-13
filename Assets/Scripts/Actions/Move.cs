using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move : ChoiceAction<Vector2Int>
{
    private List<Vector2Int> path;

    public override void Build(Vector2Int destination)
    {
        if (choice == destination) {
            return;
        }
        choice = destination;
        path = GetPath(destination);
    }

    private List<Vector2Int> GetPath(Vector2Int destination) {
        GridElement src = GetComponent<GridElement>();
        return Pathfinding.FindPath(src.GetPosition(), destination, CanWalk);
    }

    public bool CanWalk(Vector2Int pos) {
        GridElement src = GetComponent<GridElement>();
        Tile tile = src.GetBoard().GetTile(pos);
        return true; // TODO
    }

    public override bool ValidateParams(Vector2Int given)
    {
        return path != null && path.All(CanWalk);
    }

    protected override void PerformAction()
    {
        var element = GetComponent<GridElement>();
        var board = element.GetBoard();
        var destination = board.GetWorldPosition(choice);
        gameObject.transform.position = destination;
    }

    // Janky gizmos testing
    [SerializeField]
    private Vector2Int inspectorChoice;
    private void OnDrawGizmosSelected()
    {
        Build(inspectorChoice);
        var element = GetComponent<GridElement>();
        if (choice == null) {
            return;
        }
        Color color = ValidateParams(choice) ? Color.green : Color.red;
        path.ForEach(t => element.GetBoard().GizmosDrawTile(t, color));
    }
}
