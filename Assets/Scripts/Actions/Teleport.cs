using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridElement))]
public class Teleport : ChoiceAction<Vector2Int>
{
    [SerializeField]
    private Vector2Int inspectorChoice;

    private void OnDrawGizmosSelected()
    {
        choice = inspectorChoice;
        var element = GetComponent<GridElement>();
        if (choice == null) {
            return;
        }
        Color color = ValidateParams(choice) ? Color.green : Color.red;
        element.GetGrid().GizmosDrawTile(choice, color);
    }

    public override bool ValidateParams(Vector2Int given)
    {
        return true; // Sure ya lol
    }

    protected override void PerformAction()
    {
        var element = GetComponent<GridElement>();
        var board = element.GetGrid();
        var destination = board.GetWorldPosition(choice);
        gameObject.transform.position = destination;
    }
}
