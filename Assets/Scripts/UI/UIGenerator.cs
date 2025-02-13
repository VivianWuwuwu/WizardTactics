using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
UI Generator creates Async methods for different "questions"

IE, 
*/
public class UIGenerator : MonoBehaviour
{
    public static UIGenerator instance;

    public TileSelector cursor;

    void Awake() {
        instance = this;
    }

    public async Task<Vector2Int> SelectTile(Board b) {
        cursor.selection = null;
        var tcs = new TaskCompletionSource<Vector2Int>();
        StartCoroutine(WaitForSelection(tcs));
        return await tcs.Task;
    }

    private IEnumerator WaitForSelection(TaskCompletionSource<Vector2Int> tcs)
    {
        yield return new WaitUntil(() => cursor.selection.HasValue);
        tcs.SetResult(cursor.selection.Value);
    }
}
