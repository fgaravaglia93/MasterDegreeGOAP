using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PersonalityAgent))]
[CanEditMultipleObjects]
public class InheritancePersonalityAgentEditor : UnityEditor.Editor
{

    SerializedProperty big5;
    SerializedProperty mood;

    void OnEnable()
    {
        big5 = serializedObject.FindProperty("BIG 5 personality model");
        mood = serializedObject.FindProperty("mood");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(mood);
        EditorGUILayout.PropertyField(big5);
        serializedObject.ApplyModifiedProperties();
    }
}