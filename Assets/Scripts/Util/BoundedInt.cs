using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

[System.Serializable]
public class BoundedInt
{
    [SerializeField] private int min;
    [SerializeField] private int max;
    [SerializeField] private int value;

    public int Min
    {
        get => min;
        set
        {
            min = value;
            if (min > max) max = min;
            Value = Mathf.Clamp(Value, min, max);
        }
    }

    public int Max
    {
        get => max;
        set
        {
            max = value;
            if (max < min) min = max;
            Value = Mathf.Clamp(Value, min, max);
        }
    }

    public int Value
    {
        get => Mathf.Clamp(value, min, max);
        set => this.value = Mathf.Clamp(value, min, max);
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