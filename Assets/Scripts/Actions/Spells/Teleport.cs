using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Teleport : ChoiceAbility<Vector2Int>
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
        Color color = IsValid() ? Color.green : Color.red;
        element.GetBoard().GizmosDrawTile(choice, color);
    }

    protected override IEnumerator PerformAction()
    {
        var element = GetComponent<GridElement>();
        var board = element.GetBoard();
        var destination = board.GetWorldPosition(choice);
        gameObject.transform.position = destination;
        yield return null;
    }
}
