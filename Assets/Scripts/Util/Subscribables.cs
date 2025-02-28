using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;

/*
An "OrderedSubscription" is basically a Priority Queue with references to the Monobehaviors for tracing subscribers
Each subscriber sorted executed in priority order (tie breaker is subscription order)
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

/*
A SubscribableIEnumerator is an OrderedQueue that is able to zip all of its Enumerators into a single larger Coroutine

We generally use this to inject behavior into phases of a combatant's turn. IE:
[Injected burn deals damage] -> Player refreshes -> [Injected burn status tics down]
*/
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

/*
A MutatableValue describes a value that can be edited via attached mutators, while still exposing the original value

We generally use this to safely perform modifications on player stats
Priority matters here, for example a 2x multiplier should apply AFTER a +10 attack modifier
*/
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
        OrderedSubscription<T> subscriptions = GetTarget(property);
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
        OrderedSubscription<T> subscriptions = GetTarget(property);
        int subscriberCount = subscriptions?.GetSubscribers().Count ?? 0;
        return EditorGUIUtility.singleLineHeight * (1 + subscriberCount);
    }

    private OrderedSubscription<T> GetTarget(SerializedProperty property)
    {
        object parentObject = GetParentObject(property);
        return fieldInfo.GetValue(parentObject) as OrderedSubscription<T>;
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

[CustomPropertyDrawer(typeof(SubscribableIEnumerator))]
public class IEnumSubscriptionDrawer : OrderedSubscriptionDrawer<Func<IEnumerator>>{}

// DRAWERS FOR ALL TYPES OF MUTATIONS WE EXPECT TO USE
public class SubscribableMutationDrawer<T> :  OrderedSubscriptionDrawer<Func<T, T>>{}

[CustomPropertyDrawer(typeof(SubscribableMutation<bool>))]
public class SubscribableMutationBoolDrawer : SubscribableMutationDrawer<bool>{}