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
    public PathDisplay pathPreview;

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
        Debug.Log($"Awaiting input...");

        /*
        while (!keycodes.Any(k => Input.GetKeyDown(k))) {
            yield return null;
        }
        var chosen = keycodes.First(k => Input.GetKeyDown(k));
        tcs.SetResult(keycodes.ToList().IndexOf(chosen));
        */

        yield return null;
        int forcedResult = 0;
        tcs.SetResult(forcedResult);
        Debug.Log($"You chose: {choices[forcedResult]}");
    }

    public async Task<Vector2Int> SelectTile(Board b, Func<Vector2Int, bool> tileIsValid) {
        cursor.Setup(b);

        var tileSelection = new TaskCompletionSource<Vector2Int>();
        IEnumerator pollCursor()
        {
            cursor.gameObject.SetActive(true);
            while (!tileSelection.Task.IsCompleted) {
                cursor.allowSelection = tileIsValid(cursor.position);
                if (cursor.selection.HasValue) {
                    tileSelection.SetResult(cursor.selection.Value);
                    cursor.gameObject.SetActive(false);
                }
                yield return null;
            }
        }
        StartCoroutine(pollCursor());
        return await tileSelection.Task;
    }

    public async Task<Vector2Int> SelectPath(Board b, Func<Vector2Int, bool> choiceIsValid, Func<Vector2Int, List<Vector2Int>> pathGenerator) {
        Debug.Log("Opening up Path selection UI");
        cursor.Setup(b);
        var tileSelection = new TaskCompletionSource<Vector2Int>();
        IEnumerator pollCursor()
        {
            cursor.gameObject.SetActive(true);
            pathPreview.gameObject.SetActive(true);
            while (!tileSelection.Task.IsCompleted) {
                bool createdValidPath = choiceIsValid(cursor.position);
                cursor.allowSelection = createdValidPath;
                // pathPreview.isValid = createdValidPath;
                pathPreview.Path = pathGenerator(cursor.position);
                if (cursor.selection.HasValue) {
                    tileSelection.SetResult(cursor.selection.Value);
                    pathPreview.gameObject.SetActive(false);
                    cursor.gameObject.SetActive(false);
                }
                yield return null;
            }
        }
        StartCoroutine(pollCursor());
        return await tileSelection.Task;
    }
}
