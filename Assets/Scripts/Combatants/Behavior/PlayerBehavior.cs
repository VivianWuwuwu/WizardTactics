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
        await PopulateAction(chosen);
        return chosen;
        // ^ How can I validate this ...
    }

    private Action DecideAction() {
        // TODO -> Add a UI picker to decide which action to use
        return GetComponents<Action>().FirstOrDefault(null);
    }

    // TODO -> Include validation for actions?
    private async Task PopulateAction(Action action) {
        // NOOP
    }

    private async Task PopulateAction(ChoiceAction<Vector2Int> action) {
        Board board = action.GetComponent<GridElement>().GetBoard();
        var selection = await UIGenerator.instance.SelectTile(board); // This is also slightly unfortunate variable passing
        action.Build(selection);
    }
}