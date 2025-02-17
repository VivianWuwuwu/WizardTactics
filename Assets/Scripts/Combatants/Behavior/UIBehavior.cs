using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

/*
UI Behavior is responsible for creating UI to manage all decisions a combatant can make
*/
public class UIBehavior : CombatantBehavior
{
    public async override Task<BaseAction> Decide()
    {
        BaseAction chosen = await PickAction();
        Debug.Log($"Chose action {chosen.GetType().Name}");
        await Prompt((dynamic)chosen);
        return chosen;
    }

    private async Task<BaseAction> PickAction() {
        BaseAction[] choices = GetComponents<BaseAction>();
        int idx = await UIGenerator.instance.SelectTextChoice(choices.Select(a => a.GetType().Name).ToList());
        return choices[idx];
    }

    /*
    We dispatch to most specific Populate method (via dynamic)
    */
    public Task LookupUI(BaseAction action) {
        return Prompt((dynamic)action); // We
    }
    
    public Task Prompt(BaseAction action) {
        return Task.CompletedTask; // No targets means no UI is needed
    }

    // Tile selection UI
    public async Task Prompt(ChoiceAction<Vector2Int> chooseTileAction) {
        Board board = chooseTileAction.GetComponent<GridElement>().GetBoard();
        while (!chooseTileAction.IsValid()) {
            Vector2Int selected = await UIGenerator.instance.SelectTile(board, chooseTileAction.IsValid);
            chooseTileAction.choice = selected;
        }
    }

    // Pathing UI
    public async Task Prompt(PathAction choosePathAction) {
        GridElement element = choosePathAction.GetComponent<GridElement>(); 
        Board board = choosePathAction.GetComponent<GridElement>().GetBoard();
        while (!choosePathAction.IsValid()) {
            List<Vector2Int> selected = await UIGenerator.instance.SelectPath(board, element.GetPosition(), choosePathAction.IsValid);
            choosePathAction.choice = selected;
        }
    }
}