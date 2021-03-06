using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TestsRuntimeVariables)), CanEditMultipleObjects, ExecuteInEditMode]
public class DynamicVisibilityDrawer : PropertyDrawer
{
	public int m_numberOfFields=9;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		EditorGUI.BeginProperty(position, label, property);

		//string fieldName = property.FindPropertyRelative("m_testName").stringValue;
		string fieldName = ObjectNames.NicifyVariableName(label.text);
		EditorGUI.LabelField(position, fieldName, EditorStyles.boldLabel);
		EditorGUI.indentLevel++;

		Rect[] rects = new Rect[m_numberOfFields];
		for(int i = 0; i < m_numberOfFields; i++) {
			rects[i] = new Rect(position.x, position.y + 18*(i+1), position.width, 16);
		}

		int t = 0;
		float baseLabelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 225f;

		property.serializedObject.Update();
		EditorGUI.BeginChangeCheck();

		if(property.FindPropertyRelative("m_testAngle").boolValue) {
			EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("m_angleTarget"));
		}
		if(property.FindPropertyRelative("m_testDistance").boolValue) {
			EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("m_distanceTarget"));
		}
		if(property.FindPropertyRelative("m_testRaycast").boolValue) {
			EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("m_raycastTarget"));
		}
		if(property.FindPropertyRelative("m_testTag").boolValue) {
			EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("m_tagToCheck"));
		}

		if(EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUIUtility.labelWidth = baseLabelWidth;

		EditorGUI.indentLevel--;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

		m_numberOfFields = property.FindPropertyRelative("m_numberOfDynamicVariables").intValue;

		//+x for the spacing between the camps, 2 for each camp
		return EditorGUIUtility.singleLineHeight * m_numberOfFields + ((m_numberOfFields-1)*2);
	}
}
