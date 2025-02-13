using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // This interleaving of async and coroutine is a bit weird
    public async Task<int> SelectTextChoice(List<string> choices) {
        var tcs = new TaskCompletionSource<int>();
        StartCoroutine(WaitForTextChoice(choices, tcs));
        return await tcs.Task;
    }

    private IEnumerator WaitForTextChoice(List<string> choices, TaskCompletionSource<int> tcs)
    {
        KeyCode keyBase = KeyCode.Q;
        KeyCode[] keycodes = choices.Select((_, idx) => keyBase + idx).ToArray();

        string prompt = string.Join(", ", choices.Select((choice, idx) => $"{choice} - ({keycodes[idx]})"));
        Debug.Log($"Choose: {prompt}");
        while (!keycodes.Any(k => Input.GetKeyDown(k))) {
            yield return null;
        }
        var chosen = keycodes.First(k => Input.GetKeyDown(k));
        tcs.SetResult(keycodes.ToList().IndexOf(chosen));
    }


    public async Task<Vector2Int> SelectTile(Board b) {
        cursor.board = b;
        cursor.selection = null;
        var tcs = new TaskCompletionSource<Vector2Int>();
        StartCoroutine(WaitForTileSelection(tcs));
        return await tcs.Task;
    }

    private IEnumerator WaitForTileSelection(TaskCompletionSource<Vector2Int> tcs)
    {
        cursor.gameObject.SetActive(true);
        yield return new WaitUntil(() => cursor.selection.HasValue);
        tcs.SetResult(cursor.selection.Value);
        cursor.gameObject.SetActive(false);
    }
}
