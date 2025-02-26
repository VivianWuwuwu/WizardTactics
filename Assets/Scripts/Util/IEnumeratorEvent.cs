using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

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

        public Subscriber(MonoBehaviour owner, Func<IEnumerator> coroutineFunc)
        {
            this.owner = owner;
            this.coroutineFunc = coroutineFunc;
        }

        public bool IsValid() => owner != null && owner.isActiveAndEnabled;
    }

    public List<Subscriber> subscriberList = new();

    public void Subscribe(Func<IEnumerator> coroutineFunc, MonoBehaviour owner)
    {
        if (owner == null || coroutineFunc == null) return;

        var subscriber = new Subscriber(owner, coroutineFunc);
        subscriberList.Add(subscriber);
    }

    /*
    public void Unsubscribe(MonoBehaviour owner)
    {
        if (owner == null) return;

        subscriberList.RemoveAll(sub => sub == owner);
        subscriberInfo.RemoveAll(info => info.StartsWith(owner.gameObject.name));
    }
    */

    /*
    public void Invoke(MonoBehaviour invoker)
    {
        for (int i = subscriberList.Count - 1; i >= 0; i--)
        {
            var sub = subscriberList[i];

            if (!sub.IsValid())
            {
                subscriberInfo.Remove($"{sub.gameObject.name} → {sub.methodName}");
                subscriberList.RemoveAt(i);
                continue;
            }

            invoker.StartCoroutine(sub.coroutineFunc());
        }
    }

    public void Clear()
    {
        subscriberList.Clear();
        subscriberInfo.Clear();
    }
    ^ Figure these out last :3
    */
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

        // Draw each subscriber info
        for (int i = 0; i < safeEvent.subscriberList.Count; i++)
        {
            IEnumeratorEvent.Subscriber curr = safeEvent.subscriberList[i];
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
        int subscriberCount = safeEvent?.subscriberList.Count ?? 0;
        return EditorGUIUtility.singleLineHeight * (1 + subscriberCount);
    }
}