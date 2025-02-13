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
        cursor.gameObject.SetActive(false);
    }

    public async Task<Vector2Int> SelectTile(Board b) {
        cursor.board = b;
        cursor.selection = null;
        var tcs = new TaskCompletionSource<Vector2Int>();
        StartCoroutine(WaitForSelection(tcs));
        return await tcs.Task;
    }

    private IEnumerator WaitForSelection(TaskCompletionSource<Vector2Int> tcs)
    {
        cursor.gameObject.SetActive(true);
        yield return new WaitUntil(() => cursor.selection.HasValue);
        tcs.SetResult(cursor.selection.Value);
        cursor.gameObject.SetActive(false);
    }
}
