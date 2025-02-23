using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

// This is just a Bounded Int, but bound from [0 -> N]
[System.Serializable]
public class CappedInt
{
    [SerializeField] protected int max;
    [SerializeField] protected int value;

    public CappedInt(CappedInt original)
    {
        (max, value) = (original.max, original.value);
    }

    public virtual int Min {
        get => 0;
        set {} // No-Op
    }

    public int Max
    {
        get => max;
        set
        {
            max = value;
            if (max < 0) max = 0;
            Value = Mathf.Clamp(Value, 0, max);
        }
    }

    public int Value
    {
        get => value;
        set => this.value = Mathf.Clamp(Value, 0, max);
    }

    // Overload the + operator
    public static CappedInt operator +(CappedInt a, int b)
    {
        CappedInt result = new CappedInt(a);
        result.Value += b; // This will use the existing Value property
        return result;
    }

    // Overload the - operator
    public static CappedInt operator -(CappedInt a, int b)
    {
        CappedInt result = new CappedInt(a);
        result.Value -= b; // This will use the existing Value property
        return result;
    }
}

[System.Serializable]
public class BoundedInt : CappedInt
{
    [SerializeField] protected int min;
    public override int Min {
        get => min;
        set
        {
            min = value;
            if (min > max) max = min;
            Value = Mathf.Clamp(Value, min, max);
        }
    }

    public BoundedInt(BoundedInt original) : base(original)
    {
        min = original.min;
    }
}

// Plus custom property drawers for Range[] sliders
[CustomPropertyDrawer(typeof(CappedInt))]
public class CappedIntDrawer : PropertyDrawer
{
    private bool isExpanded = true;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty maxProp = property.FindPropertyRelative("max");
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        Rect foldoutRect = new Rect(position.x, position.y, position.width, lineHeight);
        isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, label);

        if (isExpanded)
        {
            Rect boxRect = new Rect(position.x, position.y + lineHeight, position.width, 2 * (lineHeight + spacing));
            GUI.Box(boxRect, "", EditorStyles.helpBox);

            Rect maxRect = new Rect(position.x + 5, position.y + 1 * (lineHeight + spacing), position.width - 10, lineHeight);
            Rect valueRect = new Rect(position.x + 5, position.y + 2 * (lineHeight + spacing), position.width - 10, lineHeight);

            maxProp.intValue = EditorGUI.IntField(maxRect, "Max", maxProp.intValue);
            if (maxProp.intValue < 0) maxProp.intValue = 0;

            valueProp.intValue = EditorGUI.IntSlider(valueRect, "Value", Mathf.Clamp(valueProp.intValue, 0, maxProp.intValue), 0, maxProp.intValue);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!isExpanded)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        return 1.75f * (lineHeight + 1) * spacing;
    }
}

[CustomPropertyDrawer(typeof(BoundedInt))]
public class BoundedIntDrawer : PropertyDrawer
{
    private bool isExpanded = true;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty minProp = property.FindPropertyRelative("min");
        SerializedProperty maxProp = property.FindPropertyRelative("max");
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        Rect foldoutRect = new Rect(position.x, position.y, position.width, lineHeight);
        isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, label);

        if (isExpanded)
        {
            Rect boxRect = new Rect(position.x, position.y + lineHeight, position.width, 3 * (lineHeight + spacing));
            GUI.Box(boxRect, "", EditorStyles.helpBox);

            Rect minRect = new Rect(position.x + 5, position.y + lineHeight + spacing, position.width - 10, lineHeight);
            Rect maxRect = new Rect(position.x + 5, position.y + 2 * (lineHeight + spacing), position.width - 10, lineHeight);
            Rect valueRect = new Rect(position.x + 5, position.y + 3 * (lineHeight + spacing), position.width - 10, lineHeight);

            minProp.intValue = EditorGUI.IntField(minRect, "Min", minProp.intValue);
            maxProp.intValue = EditorGUI.IntField(maxRect, "Max", maxProp.intValue);

            if (minProp.intValue > maxProp.intValue) maxProp.intValue = minProp.intValue;
            if (maxProp.intValue < minProp.intValue) minProp.intValue = maxProp.intValue;

            valueProp.intValue = EditorGUI.IntSlider(valueRect, "Value", Mathf.Clamp(valueProp.intValue, minProp.intValue, maxProp.intValue), minProp.intValue, maxProp.intValue);
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!isExpanded)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        return 4 * lineHeight + 3 * spacing;
    }
}