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
    public async override Task<BaseAction> Decide()
    {
        BaseAction chosen = await DecideAction();
        Debug.Log($"Chose action {chosen.GetType().Name}");
        await Populate((dynamic)chosen);
        return chosen;
    }

    private async Task<BaseAction> DecideAction() {
        BaseAction[] choices = GetComponents<BaseAction>();
        int idx = await UIGenerator.instance.SelectTextChoice(choices.Select(a => a.GetType().Name).ToList());
        return choices[idx];
    }

    /*
    We dispatch to most specific Populate method (via dynamic)
    */
    public override Task Populate(BaseAction action) {
        return Task.CompletedTask; // No-Op
    }

    public async Task Populate(Teleport action) {
        Board board = action.GetComponent<GridElement>().GetBoard();
        var selection = await UIGenerator.instance.SelectTile(board);
        action.Build(selection);
    }
}