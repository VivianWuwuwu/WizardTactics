using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Runtime.InteropServices;

/*
An "IEnumeratorEvent" is effectively an Event, but with IEnumerator subscribers rather than functions

Each subscriber is executed in order. This allows us to enqueue a number of subscribers, and invoke each in order


EXAMPLE USEAGE:
private IEnumeratorEvent myEvent = new IEnumeratorEvent();

private void Start()
{
    myEvent += MyCoroutine;
    myEvent += AnotherCoroutine;

    StartCoroutine(myEvent.Invoke());

    myEvent -= MyCoroutine; // Unsubscribing an event
}


(We'll add these to the combatant to facilitate Refresh, Attack funcs that statuses can hook into)
*/

[Serializable]
public class IEnumeratorEvent
{
    public class Subscriber
    {
        public MonoBehaviour owner;
        public Func<IEnumerator> coroutineFunc;
        public readonly int priority;

        public Subscriber(MonoBehaviour owner, Func<IEnumerator> coroutineFunc, int priority)
        {
            this.owner = owner;
            this.coroutineFunc = coroutineFunc;
            this.priority = priority;
        }

        public bool IsValid() => owner != null && owner.isActiveAndEnabled;
    }

    private List<Subscriber> subscriberList = new();

    public void Subscribe(Func<IEnumerator> coroutineFunc, MonoBehaviour owner, int priority = 0)
    {
        if (owner == null || coroutineFunc == null) return;

        var subscriber = new Subscriber(owner, coroutineFunc, priority);
        subscriberList.Add(subscriber);
    }

    public void Unsubscribe(Func<IEnumerator> coroutineFunc)
    {
        subscriberList.RemoveAll(p => p.coroutineFunc == coroutineFunc);
    }

    public void Unsubscribe(MonoBehaviour owner)
    {
        subscriberList.RemoveAll(p => p.owner == owner);
    }

    public List<Subscriber> GetSubscribers() {
        return subscriberList.OrderByDescending(s => s.priority).ToList();
    }

    public IEnumerator Invoke()
    {
        var ordered = GetSubscribers();
        while (ordered.Any()) {
            Subscriber curr = ordered.First();
            ordered.RemoveAt(0);

            if (!curr.IsValid()) {
                continue;
            }
            yield return curr.coroutineFunc();
        }
    }

    private void RemoveDestroyed() {
        subscriberList.RemoveAll(p => p.owner == null);
    }

    public void Clear()
    {
        subscriberList.Clear();
    }
}


// AHAHAHA  NO WAY THIS WORKS PERFECTLY HAHAAHAAA
[CustomPropertyDrawer(typeof(IEnumeratorEvent))]
public class IEnumeratorEventDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the target object (SafeEvent) from SerializedProperty
        IEnumeratorEvent safeEvent = fieldInfo.GetValue(property.serializedObject.targetObject) as IEnumeratorEvent;

        if (safeEvent == null)
        {
            EditorGUI.LabelField(position, label.text, "SafeEvent is null");
            return;
        }

        // Draw the label
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, label.text, EditorStyles.boldLabel);


        var subscribers = safeEvent.GetSubscribers();

        // Draw each subscriber info
        for (int i = 0; i < subscribers.Count; i++)
        {
            IEnumeratorEvent.Subscriber curr = subscribers[i];
            position.y += EditorGUIUtility.singleLineHeight;

            Rect labelRect = new(position.x, position.y, position.width * 0.7f, position.height);
            Rect objectFieldRect = new(position.x + position.width * 0.72f, position.y, position.width * 0.25f, position.height);
            EditorGUI.LabelField(position, $"[{curr.coroutineFunc.Method.Name}]");
            EditorGUI.ObjectField(objectFieldRect, curr.owner, typeof(MonoBehaviour), true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        IEnumeratorEvent safeEvent = fieldInfo.GetValue(property.serializedObject.targetObject) as IEnumeratorEvent;
        int subscriberCount = safeEvent?.GetSubscribers().Count ?? 0;
        return EditorGUIUtility.singleLineHeight * (1 + subscriberCount);
    }
}