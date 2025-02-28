using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoveDamage))]
[RequireComponent(typeof(Timer))]
public class Shocked : CombatantStatus {
    public static IEnumerator Conduct(Water w, Shocked _) {
        GridElement location = w.Parent.GetComponent<GridElement>();
        Board b = location.GetBoard();

        List<Vector2Int> adjacents = new List<Vector2Int>{Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right};
        List<Combatant> targets = adjacents
            .Select(v => location.GetPosition() + v)
            .Select(v => b.GetTile(v))
            .Select(t => t.GetCombatant())
            .NotNull()
            .ToList();

        w.Destroy();
        foreach (Combatant c in targets) {
            ResourceLocator.Instance.Statuses.CreateStatus<Shocked>(c);
            yield return null;
        }
    }
}