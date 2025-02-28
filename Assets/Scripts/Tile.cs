using System.Collections.Generic;
using System.Linq;

public class Tile
{
    public ISet<GridElement> elements;

    public Tile(ISet<GridElement> elements) {
        this.elements = elements;
    }

    // Kindaaaaa scuffed
    public Combatant GetCombatant() {
        return elements.Select(e => e.GetComponent<Combatant>()).Where(c => c != null).FirstOrDefault();
    }

    public bool Occupied() {
        return false;
    }
}
