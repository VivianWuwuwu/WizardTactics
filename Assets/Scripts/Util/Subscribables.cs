using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Xml.Schema;

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
        public bool forceStop; // If a subscriber has a "Stop", we just cut-off the queue at this subscriber. We can use this to override
        public readonly int priority;

        public Subscriber(MonoBehaviour owner, ItemType item, int priority, bool forceStop)
        {
            this.owner = owner;
            this.item = item;
            this.priority = priority;
            this.forceStop = forceStop;
        }

        public bool IsValid() => owner != null && owner.isActiveAndEnabled;
    }

    private List<Subscriber> subscriberList;
    public MonoBehaviour owner;

    public OrderedSubscription(MonoBehaviour owner) {
        subscriberList = new List<Subscriber>();
        this.owner = owner;
    }

    public void Subscribe(ItemType item, MonoBehaviour owner, int priority = 0, bool forceStop = false)
    {
        if (owner == null || item == null) return;

        var subscriber = new Subscriber(owner, item, priority, forceStop);
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
        if (subscriberList == null) {
            return null;
        }
        Sync();
        return subscriberList.OrderByDescending(s => s.priority).ToList();
    }

    // Returns all subscribers, ordered, halting at the first "Stop"
    public List<Subscriber> GetActiveSubscribers() {
        if (GetSubscribers() == null) {
            return null;
        }
        var trimmed = new List<Subscriber>();
        foreach (var sub in GetSubscribers()) {
            trimmed.Add(sub);
            if (sub.forceStop) {
                return trimmed;
            }
        }
        return trimmed;
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
    public SubscribableIEnumerator(MonoBehaviour owner) : base(owner){}
    public SubscribableIEnumerator(MonoBehaviour owner, Func<IEnumerator> basis) : base(owner){
        this.Subscribe(basis, owner);
    }

    public IEnumerator Invoke()
    {
        var ordered = GetActiveSubscribers();
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
    public SubscribableMutation(MonoBehaviour owner) : base(owner){}

    public T Mutate(T original)
    {
        var ordered = GetActiveSubscribers();
        if (ordered == null) {
            return original;
        }
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
    public MutatableValue(MonoBehaviour owner, T basis) {
        Mutations = new SubscribableMutation<T>(owner);
        this.basis = basis;
    }
}

/*
PROPERTY DRAWERS TO MAKE THESE QUEUES LOOK NICE IN THE EDITOR

Ok all of this is nonsense spat out by ChatGPT
We're just using this to give all these framework classes a nice display in the Unity UI. Don't worry about it hehe

DO NOT BOTHER LOOKING BELOW
*/
// This drawer works for any subscription type. We'll need to overload with specific drawer types if we want em to show up in the UI
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
        } else if (subscriptions.GetSubscribers() == null) {
            EditorGUI.LabelField(position, label.text, "Subscriptions not initialized");
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

            // "X" Label Position
            if (curr.forceStop) {
                float xSize = position.height; // Square size based on row height
                Rect xLabelRect = new Rect(objectFieldRect.xMax + 5, position.y, xSize, xSize);

                // Create a custom style for the "X"
                GUIStyle xStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.red },
                    alignment = TextAnchor.MiddleCenter
                };
                GUI.Label(xLabelRect, EditorGUIUtility.IconContent("console.erroricon"), xStyle);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        OrderedSubscription<T> subscriptions = GetTarget(property);
        var subs = subscriptions?.GetSubscribers();
        int subscriberCount = 0;
        if (subs != null) {
            subscriberCount = subs.Count;
        }
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


// AND THE MUTATIONS I GUESS? UGH

[CustomPropertyDrawer(typeof(MutatableValue<>), true)]
public class MutatableValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect fieldRect = new Rect(position.x, position.y, position.width - 30, position.height);

        string value = GetMutatedValue(property);
        EditorGUI.PropertyField(fieldRect, property, label, true);
        if (property.isExpanded) {
            // It'd be so..... cool.... to prettify this l8r
            Rect buttonRect = new Rect(fieldRect.x + 12, fieldRect.y + fieldRect.height - 17, 100, 20);
            if (GUI.Button(buttonRect, "Log value")) {
                Debug.Log($"Mutated Value got Value:\n{GetMutatedValue(property)}");
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUI.GetPropertyHeight(property, label, true);
        if (property.isExpanded) {
            height += 20;
        }
        return height;
    }

    // Absolute reflection black magic spat out here by chatgpt
    private string GetMutatedValue(SerializedProperty property)
    {
        object parentObject = GetParentObject(property);
        if (parentObject == null) return null;

        object mutatableValue = fieldInfo.GetValue(parentObject);
        if (mutatableValue == null) return null;

        PropertyInfo getProperty = mutatableValue.GetType().GetProperty("Get");
        object mutatedValue = getProperty?.GetValue(mutatableValue);
        return $"{mutatedValue}";
    }

    private object GetParentObject(SerializedProperty property)
    {
        object target = property.serializedObject.targetObject;
        string[] elements = property.propertyPath.Split('.');
        for (int i = 0; i < elements.Length - 1; i++)
        {
            FieldInfo field = target.GetType().GetField(elements[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return null;
            target = field.GetValue(target);
        }
        return target;
    }
}
