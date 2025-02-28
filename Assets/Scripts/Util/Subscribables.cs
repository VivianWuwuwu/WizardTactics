using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Runtime.InteropServices;

/*
An "SubscribableIEnumerator" is effectively an Event, but with IEnumerator subscribers rather than functions

Each subscriber is executed in order. This allows us to enqueue a number of subscribers, and invoke each in order


EXAMPLE USEAGE:
private SubscribableIEnumerator myEvent = new SubscribableIEnumerator();

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
public class OrderedSubscription<ItemType> {
    public class Subscriber
    {
        public MonoBehaviour owner;
        public ItemType item;
        public readonly int priority;

        public Subscriber(MonoBehaviour owner, ItemType item, int priority)
        {
            this.owner = owner;
            this.item = item;
            this.priority = priority;
        }

        public bool IsValid() => owner != null && owner.isActiveAndEnabled;
    }

    private List<Subscriber> subscriberList = new();

    public void Subscribe(ItemType item, MonoBehaviour owner, int priority = 0)
    {
        if (owner == null || item == null) return;

        var subscriber = new Subscriber(owner, item, priority);
        subscriberList.Add(subscriber);
    }

    public void Unsubscribe(ItemType item)
    {
        subscriberList.RemoveAll(p => p.item.Equals(item));
    }

    public void Unsubscribe(MonoBehaviour owner)
    {
        subscriberList.RemoveAll(p => p.owner == owner);
    }

    public List<Subscriber> GetSubscribers() {
        return subscriberList.OrderByDescending(s => s.priority).ToList();
    }

    public void Sync() {
        subscriberList.RemoveAll(p => p.owner == null);
    }

    public void Clear()
    {
        subscriberList.Clear();
    }
}

[Serializable]
public class SubscribableIEnumerator : OrderedSubscription<Func<IEnumerator>>
{
    public IEnumerator Invoke()
    {
        var ordered = GetSubscribers();
        while (ordered.Any()) {
            Subscriber curr = ordered.First();
            ordered.RemoveAt(0);

            if (!curr.IsValid()) {
                continue;
            }
            yield return curr.item();
        }
    }
}

[Serializable]
public class SubscribableMutation<T> : OrderedSubscription<Func<T, T>>
{
    public T Mutate(T original)
    {
        var ordered = GetSubscribers();
        while (ordered.Any()) {
            Subscriber curr = ordered.First();
            ordered.RemoveAt(0);
            if (!curr.IsValid()) {
                continue;
            }
            Func<T, T> mutate = curr.item;
            original = mutate(original);
        }
        return original;
    }
}

[Serializable]
public class MutatableValue<T>
{
    public T basis;
    public T Get => Mutations.Mutate(basis);
    public SubscribableMutation<T> Mutations;
    public MutatableValue(T basis) {
        this.basis = basis;
    }
}

// Note that this works for any subscription type. We'll need to overload with specific drawer types if we want em to show up in the UI
public class OrderedSubscriptionDrawer<T> : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the target object (SafeEvent) from SerializedProperty
        OrderedSubscription<T> subscriptions = fieldInfo.GetValue(property.serializedObject.targetObject) as OrderedSubscription<T>;

        if (subscriptions == null)
        {
            EditorGUI.LabelField(position, label.text, "Subscriptions is null");
            return;
        }

        // Draw the label
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, label.text, EditorStyles.boldLabel);


        var subscribers = subscriptions.GetSubscribers();

        // Draw each subscriber info
        for (int i = 0; i < subscribers.Count; i++)
        {
            OrderedSubscription<T>.Subscriber curr = subscribers[i];
            position.y += EditorGUIUtility.singleLineHeight;

            Rect objectFieldRect = new(position.x, position.y, position.width * 0.25f, position.height);
            EditorGUI.ObjectField(objectFieldRect, curr.owner, typeof(MonoBehaviour), true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SubscribableIEnumerator safeEvent = fieldInfo.GetValue(property.serializedObject.targetObject) as SubscribableIEnumerator;
        int subscriberCount = safeEvent?.GetSubscribers().Count ?? 0;
        return EditorGUIUtility.singleLineHeight * (1 + subscriberCount);
    }
}

[CustomPropertyDrawer(typeof(SubscribableIEnumerator))]
public class IEnumSubscriptionDrawer : OrderedSubscriptionDrawer<Func<IEnumerator>>{}