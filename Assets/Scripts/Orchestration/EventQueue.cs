using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using OneOf;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Diagnostics.Tracing;

/*
This is somewhat involved so uhhh strap in lol
*/

[Serializable]
public class ActionQueue {
    /*
    An ActionQueue consists of elements that can either be:
    -> An Event (an IEnumerator)
    -> A Subqueue

    Whenever we hit a subqueue we unpack it then add all its events into the queue where that subqueue was
    */
    public List<OneOf<Subscriber<Func<IEnumerator>>, Subscriber<Func<ActionQueue>>>> events;

    public void Push(Func<IEnumerator> action, MonoBehaviour owner, int priority = 0, bool forceStop = false) {
        var element = new Subscriber<Func<IEnumerator>>(owner, action, priority, forceStop);
        PushElement(element);
    }

    public void Push(Func<ActionQueue> subqueue, MonoBehaviour owner, int priority = 0) {
        var element = new Subscriber<Func<ActionQueue>>(owner, subqueue, priority, false);
        PushElement(element);
    }

    public bool Any() {
        return events.Any();
    }

    public Subscriber<Func<IEnumerator>> Pop() {
        var got = Peek();
        if (events.Any()) {
            events.RemoveAt(0); 
        }
        return got;
    }

    public Subscriber<Func<IEnumerator>> Peek() {
        if (!events.Any()) {
            return null;
        }
        var front = events[0];

        return front.Match(
            action => action,
            subqueue => {
                events.RemoveAt(0);
                UnpackSubqueue(subqueue);
                return Peek();
            }
        );
    }

    private void UnpackSubqueue(Subscriber<Func<ActionQueue>> subqueue) {
        /*
        Every item extracted from a subqueue replaces that subqueue's position. Therefore we make some edits:
            -> Priority is the priority of the parent queue
            -> ForceStop is false

        The general rationale is that these params should only affect the ordering of the local queue, not the containing queue
        */
        Stack<Subscriber<Func<IEnumerator>>> extracted = new Stack<Subscriber<Func<IEnumerator>>>();
        var packed = subqueue.item(); // Get the queue
        while(packed.Any()) {
            Subscriber<Func<IEnumerator>> extraction = packed.Pop();
            extracted.Push(new Subscriber<Func<IEnumerator>>(extraction.owner, extraction.item, subqueue.priority, false)); 
        }
        while (extracted.Any()) {
            PushElement(extracted.Pop());
        }
    }

    private void PushElement(OneOf<Subscriber<Func<IEnumerator>>, Subscriber<Func<ActionQueue>>> newItem)
    {
        // Unfortunate spaghetti boilerplate but both cases in our switch have a priority :3
        Func<OneOf<Subscriber<Func<IEnumerator>>, Subscriber<Func<ActionQueue>>>, int> getPriority = (item) =>
            item.Match(
                action => action.priority,
                subqueue => subqueue.priority
            );

        int index = events.FindIndex(item => getPriority(item) == getPriority(newItem));
        if (index == -1)
            index = 0; // Just insert at start if no higher priority elements found
        events.Insert(index, newItem);
    }
}

