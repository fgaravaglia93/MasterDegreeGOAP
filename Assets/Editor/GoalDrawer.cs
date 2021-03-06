using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Goal)), CanEditMultipleObjects, ExecuteInEditMode]
public class GoalDrawer : PropertyDrawer
{
    public int m_numberOfFields = 1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        Rect[] rects = new Rect[m_numberOfFields];
        for(int i = 0; i < m_numberOfFields; i++) {
            rects[i] = new Rect(position.x+125, position.y + 18 * i, position.width, 16);
        }

        int t = 0;

        property.serializedObject.Update();
        //EditorGUI.BeginChangeCheck();

        EditorGUI.LabelField(rects[t++], label);


        //if(EditorGUI.EndChangeCheck())
        //    property.serializedObject.ApplyModifiedProperties();

      

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

        m_numberOfFields = 1;

        //+x for the spacing between the camps, 2 for each camp
        return EditorGUIUtility.singleLineHeight * m_numberOfFields + ((m_numberOfFields - 1) * 2);
    }
}
