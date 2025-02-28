using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// No-Op status - only used for combining
public class Water : CombatantStatus {
    public static IEnumerator Extinguish(Water w, Burn b) {
        w.Destroy();
        b.Destroy();
        // Play smoke animation here?
        yield return null;
    }

    public static IEnumerator Conduct(Water w, Electrify e) {
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
            // electrify target (play an animation?)
            yield return null;
        }
    }
}