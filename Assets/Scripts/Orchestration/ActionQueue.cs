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
using System.Reflection;

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
    public List<OneOf<Subscriber<Func<IEnumerator>>, Subscriber<ActionQueue>>> events;

    public ActionQueue() {
        this.events = new List<OneOf<Subscriber<Func<IEnumerator>>, Subscriber<ActionQueue>>>();
    }

    public ActionQueue(ActionQueue other) {
        this.events = new List<OneOf<Subscriber<Func<IEnumerator>>, Subscriber<ActionQueue>>>(other.events);
    }

    public void Push(Func<IEnumerator> action, MonoBehaviour owner, int priority = 0, bool forceStop = false) {
        var element = new Subscriber<Func<IEnumerator>>(owner, action, priority, forceStop);
        PushElement(element);
    }

    public void Push(ActionQueue subqueue, MonoBehaviour owner, int priority = 0) {
        var element = new Subscriber<ActionQueue>(owner, subqueue, priority, false);
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

    private void UnpackSubqueue(Subscriber<ActionQueue> subqueue) {
        /*
        Every item extracted from a subqueue replaces that subqueue's position. Therefore we make some edits:
            -> Priority is the priority of the parent queue
            -> ForceStop is false

        The general rationale is that these params should only affect the ordering of the local queue, not the containing queue
        */
        Debug.Log("Unpacking subqueue...");

        Stack<Subscriber<Func<IEnumerator>>> extracted = new Stack<Subscriber<Func<IEnumerator>>>();
        var packed = new ActionQueue(subqueue.item); // Get a local copy of the queue
        while(packed.Any()) {
            Subscriber<Func<IEnumerator>> extraction = packed.Pop();
            extracted.Push(new Subscriber<Func<IEnumerator>>(extraction.owner, extraction.item, subqueue.priority, false)); 
        }
        while (extracted.Any()) {
            PushElement(extracted.Pop());
        }
    }

    private void PushElement(OneOf<Subscriber<Func<IEnumerator>>, Subscriber<ActionQueue>> newItem)
    {
        // Unfortunate spaghetti boilerplate but both cases in our switch have a priority :3
        Func<OneOf<Subscriber<Func<IEnumerator>>, Subscriber<ActionQueue>>, int> getPriority = (item) =>
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

/*
PROPERTY DRAWERS TO MAKE THESE QUEUES LOOK NICE IN THE EDITOR

Ok all of this is nonsense spat out by ChatGPT
We're just using this to give all these framework classes a nice display in the Unity UI. Don't worry about it hehe

DO NOT BOTHER LOOKING BELOW
*/
[CustomPropertyDrawer(typeof(ActionQueue))]
public class ActionQueueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the target object (SafeEvent) from SerializedProperty
        ActionQueue actionQueue = GetTarget(property);
        if (actionQueue == null)
        {
            EditorGUI.LabelField(position, label.text, "Subscriptions is null");
            return;
        }

        // Draw the label
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, label.text, EditorStyles.boldLabel);

        // Create a button below the property field
        Rect buttonRect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(property), position.width, 30);
        if (GUI.Button(buttonRect, "Step"))
        {
            var curr = actionQueue.Pop();
            var action = curr.item();
            while (action.MoveNext()) {} // Evaluate the action :3
        }
        position.y += buttonRect.height;

        var subscribers = actionQueue.events;
        // Draw each subscriber info
        for (int i = 0; i < subscribers.Count; i++)
        {
            position.y += EditorGUIUtility.singleLineHeight;
            var curr = subscribers[i];
            curr.Switch(
                action => DrawSubscriber(action, position),
                subqueue => DrawSubscriber(subqueue, position)
            );
        }
    }

    private void DrawSubscriber(Subscriber curr, Rect position) {
        Rect objectFieldRect = new(position.x, position.y, position.width * 0.25f, position.height);
        EditorGUI.ObjectField(objectFieldRect, curr.owner, typeof(MonoBehaviour), true);

        // "X" Label Position
        Rect labelRect = new Rect(objectFieldRect.xMax + 5, position.y, position.height, position.height);
        if (!curr.owner.isActiveAndEnabled) {
            GUIStyle skipStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.gray },
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(labelRect, EditorGUIUtility.IconContent("console.erroricon"), skipStyle);
        } else if (curr.forceStop) {
            GUIStyle xStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.red },
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(labelRect, EditorGUIUtility.IconContent("console.erroricon"), xStyle);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ActionQueue queue = GetTarget(property);
        var subs = queue?.events;
        int subscriberCount = 0;
        if (subs != null) {
            subscriberCount = subs.Count;
        }
        return EditorGUIUtility.singleLineHeight * (1 + subscriberCount);
    }

    private ActionQueue GetTarget(SerializedProperty property)
    {
        object parentObject = GetParentObject(property);
        return fieldInfo.GetValue(parentObject) as ActionQueue;
    }

    private object GetParentObject(SerializedProperty property)
    {
        string path = property.propertyPath;
        object target = property.serializedObject.targetObject;

        string[] elements = path.Split('.');
        for (int i = 0; i < elements.Length - 1; i++) // Traverse to the parent object
        {
            var type = target.GetType();
            var field = type.GetField(elements[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null) return null;
            target = field.GetValue(target);
        }
        return target;
    }
}