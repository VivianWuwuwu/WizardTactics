using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTester : MonoBehaviour
{

    private ActionQueue subqueue;
    public ActionQueue actions;

    // Start is called before the first frame update
    void Awake()
    {
        ConfigureQueue();
        
    }

    // Update is called once per frame
    void OnValidate()
    {
        ConfigureQueue();
        
    }

    private void ConfigureQueue() {
        actions = new ActionQueue();
        actions.Push(SayBark, this);
        actions.Push(SayMeow, this);
        actions.Push(SayHi, this);

        subqueue = new ActionQueue();
        subqueue.Push(SayMeow, this);
        subqueue.Push(SayHi, this);
        subqueue.Push(SayBark, this);
        subqueue.Push(SayMeow, this);

        actions.Push(subqueue, this);
        actions.Push(SayBark, this);
    }

    public IEnumerator SayBark() {
        Debug.Log("Bark!");
        yield return null;
    }

    public IEnumerator SayMeow() {
        Debug.Log("Meow!");
        yield return null;
    }

    public IEnumerator SayHi() {
        Debug.Log("Hi!");
        yield return null;
    }
}
