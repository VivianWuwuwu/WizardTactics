using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

/*
Player Behavior is responsible for creating UI to manage all decisions a combatant can make
*/
public class PlayerBehavior : CombatantBehavior
{
    public async override Task<Action> Decide()
    {
        Action chosen = DecideAction();
        await Populate((dynamic)chosen);
        return chosen;
    }

    private Action DecideAction() {
        // TODO -> Add a UI picker to decide which action to use
        return GetComponents<Action>().First();
    }

    /*
    We dispatch to most specific Populate method (via dynamic)
    */
    public override Task Populate(Action action) {
        Debug.Log("No-op populating");
        return Task.CompletedTask; // No-Op
    }

    public async Task Populate(Teleport action) {
        Debug.Log("Populating teleport");
        Board board = action.GetComponent<GridElement>().GetBoard();
        var selection = await UIGenerator.instance.SelectTile(board);
        action.Build(selection);
    }
}