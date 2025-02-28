using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusRegistry")]
public class StatusRegistry : ScriptableObject {
    public List<GameObject> prefabs;

    public T CreateStatus<T>(Combatant target) where T : CombatantStatus {
        T prefab = FindPrefab<T>();
        if (prefab == null) {
            return null;
        }
        return Instantiate(prefab, target.transform); // This instantiates the entire prefab, then returns the status commponent
    }

    private T FindPrefab<T>() where T : CombatantStatus {
        return prefabs.Select(p => p.GetComponent<T>()).NotNull().FirstOrDefault();
    }

    public void OnValidate() {
        List<GameObject> validPrefabs = prefabs.Where(p => p.GetComponent<CombatantStatus>() != null).ToList();
        foreach (var invalid in prefabs.Except(validPrefabs)) {
            Debug.Log($"Removing invalid prefab: {invalid.name}");
        }
        prefabs = validPrefabs;

        // Filter out duplicate prefab types as well (IE two conflicting registry entries for <Burn>)
        var types = new HashSet<Type>();
        for (int i = prefabs.Count - 1; i >= 0; i--) {
            CombatantStatus curr = prefabs[i].GetComponent<CombatantStatus>();
            Type statusType = curr.GetType();
            if (types.Contains(statusType)) {
                Debug.Log($"Removing duplicate prefab: {curr.gameObject.name}");
                prefabs.RemoveAt(i);
            }
            types.Add(statusType);
        }
    }
}

[CustomEditor(typeof(StatusRegistry))]
public class StatusRegistryEditor : Editor {
    public override void OnInspectorGUI() {
        StatusRegistry registry = (StatusRegistry)target;

        if (registry.prefabs == null)
            registry.prefabs = new List<GameObject>();

        // Collect unique status names
        HashSet<string> statusNames = new HashSet<string>();
        foreach (var prefab in registry.prefabs) {
            if (prefab != null) {
                CombatantStatus status = prefab.GetComponent<CombatantStatus>();
                if (status != null) {
                    statusNames.Add($"<{status.GetType().Name}>");
                }
            }
        }

        // Display unique status names
        EditorGUILayout.LabelField("Registered Status Types:", EditorStyles.boldLabel);
        if (statusNames.Count > 0) {
            EditorGUILayout.HelpBox(string.Join(", ", statusNames), MessageType.Info);
        } else {
            EditorGUILayout.HelpBox("No valid status types found.", MessageType.Warning);
        }

        EditorGUILayout.Space();

        // Draw the prefab list
        SerializedProperty prefabsProperty = serializedObject.FindProperty("prefabs");
        EditorGUILayout.PropertyField(prefabsProperty, new GUIContent("Status Prefabs"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
