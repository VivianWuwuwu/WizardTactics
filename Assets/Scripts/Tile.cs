using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private ISet<GridElement> elements;

    public Tile(ISet<GridElement> elements) {
        this.elements = elements;
    }

    public bool Occupied() {
        return false;
    }
}
