using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

/*
An "OrderedEditor" is basically a Priority Queue with references to the Monobehaviors for tracing subscribers
Each subscriber sorted executed in priority order (tie breaker is subscription order)
*/
public class Subscriber {
    public MonoBehaviour owner;
    public bool forceStop; // If a subscriber has a "Stop", we just cut-off the queue at this subscriber. We can use this to override
    public readonly int priority;

    protected Subscriber(MonoBehaviour owner, int priority, bool forceStop)
    {
        this.owner = owner;
        this.priority = priority;
        this.forceStop = forceStop;
    }

    public bool IsValid() => owner != null && owner.isActiveAndEnabled;
}

public class Subscriber<T> : Subscriber
{
    public T item;
    public Subscriber(MonoBehaviour owner, T item, int priority, bool forceStop) : base(owner, priority, forceStop)
    {
        this.item = item;
    }
}

[Serializable]
public class OrderedEditor<ItemType> {
    protected List<Subscriber<ItemType>> subscriberList;
    public MonoBehaviour owner;

    public OrderedEditor(MonoBehaviour owner) {
        subscriberList = new List<Subscriber<ItemType>>();
        this.owner = owner;
    }

    public void SubscribeEdit(ItemType item, MonoBehaviour owner, int priority = 0, bool forceStop = false)
    {
        if (owner == null || item == null) return;

        var subscriber = new Subscriber<ItemType>(owner, item, priority, forceStop);
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

    public List<Subscriber<ItemType>> GetSubscribers() {
        if (subscriberList == null) {
            return null;
        }
        Sync();
        return subscriberList.OrderByDescending(s => s.priority).ToList();
    }

    // Returns all subscribers, ordered, halting at the first "Stop"
    public List<Subscriber<ItemType>> GetActiveSubscribers() {
        Sync();
        if (GetSubscribers() == null) {
            return null;
        }
        var trimmed = new List<Subscriber<ItemType>>();
        foreach (var sub in GetSubscribers()) {
            trimmed.Add(sub);
            if (sub.forceStop) {
                return trimmed;
            }
        }
        return trimmed;
    }

    private void Sync() {
        subscriberList.RemoveAll(p => p.owner == null);
    }

    public void Clear()
    {
        subscriberList.Clear();
    }
}

/*
A SubscribableIEnumerator is an OrderedEditor that is able to zip all of its Enumerators into a single larger Coroutine

We generally use this to inject behavior into phases of a combatant's turn. IE:
[Injected burn deals damage] -> Player refreshes -> [Injected burn status tics down]
*/
[Serializable]
public class EditableIEnumerator : OrderedEditor<Func<IEnumerator>>
{
    public EditableIEnumerator(MonoBehaviour owner) : base(owner){}
    public EditableIEnumerator(MonoBehaviour owner, Func<IEnumerator> basis) : base(owner){
        this.SubscribeEdit(basis, owner);
    }

    public IEnumerator Invoke()
    {
        var ordered = GetActiveSubscribers();
        while (ordered.Any()) {
            var curr = ordered.First();
            ordered.RemoveAt(0);
            if (!curr.IsValid()) {
                continue;
            }
            yield return curr.item();
        }
    }
}

[Serializable]
public class EditableMutation<T> : OrderedEditor<Func<T, T>>
{
    public EditableMutation(MonoBehaviour owner) : base(owner){}

    public T Mutate(T original)
    {
        var ordered = GetActiveSubscribers();
        if (ordered == null) {
            return original;
        }
        while (ordered.Any()) {
            var curr = ordered.First();
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
public class EditableValue<T>
{
    public T basis;
    public T Evaluate => Mutations.Mutate(basis);
    public EditableMutation<T> Mutations;
    public EditableValue(MonoBehaviour owner, T basis) {
        Mutations = new EditableMutation<T>(owner);
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
        OrderedEditor<T> subscriptions = GetTarget(property);
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
            var curr = subscribers[i];
            position.y += EditorGUIUtility.singleLineHeight;

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
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        OrderedEditor<T> subscriptions = GetTarget(property);
        var subs = subscriptions?.GetSubscribers();
        int subscriberCount = 0;
        if (subs != null) {
            subscriberCount = subs.Count;
        }
        return EditorGUIUtility.singleLineHeight * (1 + subscriberCount);
    }

    private OrderedEditor<T> GetTarget(SerializedProperty property)
    {
        object parentObject = GetParentObject(property);
        return fieldInfo.GetValue(parentObject) as OrderedEditor<T>;
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

[CustomPropertyDrawer(typeof(EditableIEnumerator))]
public class IEnumSubscriptionDrawer : OrderedSubscriptionDrawer<Func<IEnumerator>>{}

// DRAWERS FOR ALL TYPES OF MUTATIONS WE EXPECT TO USE
public class SubscribableMutationDrawer<T> :  OrderedSubscriptionDrawer<Func<T, T>>{}

[CustomPropertyDrawer(typeof(EditableMutation<bool>))]
public class SubscribableMutationBoolDrawer : SubscribableMutationDrawer<bool>{}


// AND THE MUTATIONS I GUESS? UGH

[CustomPropertyDrawer(typeof(EditableValue<>), true)]
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
