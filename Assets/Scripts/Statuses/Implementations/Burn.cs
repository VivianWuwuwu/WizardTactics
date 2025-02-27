using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Burn : CombatantStatus
{
    public void Setup(int duration, int damage) {
        // ....
    }

    public void Stack() {
        FindObjectsOfType<Burn>().Where(o => o != this);
    }
}
// First idea for prefab sorting is to just do it by name. This is ok for stacking but otherwise AWFUL.
// Can we like, use interfaces to make a builder. IFace for has duration, etc, etc
// nah this is sooooo overcooked
// I do think having these container classes is kinda necessary to tie it all together tho