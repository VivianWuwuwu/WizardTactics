using System;
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
        var tileSelection = new TaskCompletionSource<Vector2Int>();
        IEnumerator pollCursor()
        {
            cursor.gameObject.SetActive(true);
            yield return new WaitUntil(() => cursor.selection.HasValue);
            tileSelection.SetResult(cursor.selection.Value);
            cursor.gameObject.SetActive(false);
        }
        StartCoroutine(pollCursor());
        return await tileSelection.Task;
    }

    public async Task<Vector2Int> SelectPath(Board b, Func<Vector2Int, List<Vector2Int>> pathGenerator) {
        cursor.board = b;
        cursor.selection = null;
        var tileSelection = new TaskCompletionSource<Vector2Int>();

        Func<Vector2Int, List<Vector2Int>> fastPathGen = Pathfinding.Memoize(pathGenerator);

        IEnumerator pollCursor()
        {
            cursor.gameObject.SetActive(true);
            while (!tileSelection.Task.IsCompleted) {
                List<Vector2Int> path = fastPathGen(cursor.position);
                if (path != null) {
                    // draw a path. Idk how yet...
                    if (cursor.selection.HasValue) {
                        tileSelection.SetResult(cursor.selection.Value);
                        cursor.gameObject.SetActive(false);
                    }
                }
                yield return null;
            }
        }
        StartCoroutine(pollCursor());
        return await tileSelection.Task;
    }
}
