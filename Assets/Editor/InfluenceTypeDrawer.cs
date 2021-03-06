using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InfluenceType)), CanEditMultipleObjects, ExecuteInEditMode]
public class InfluenceTypeDrawer : PropertyDrawer
{
	public int m_numberOfFields = 4;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		Rect[] rects = new Rect[m_numberOfFields];
		for(int i = 0; i < m_numberOfFields; i++) {
			rects[i] = new Rect(position.x, position.y + 18 * i, position.width, 16);
		}

		int t = 0;

		property.serializedObject.Update();
		EditorGUI.BeginChangeCheck();

		EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("response"));
		EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("responseType"));

		switch((Response)property.FindPropertyRelative("response").intValue) {
			case Response.CostManipulation:
				EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("costIncrement"));
				break;
			case Response.ActionManipulation:
				EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("actionType"));
				break;
			case Response.GoalManipulation:
				EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("goal"));
				break;
            case Response.PathfindingManipulation:
                //EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("labourer"));
                EditorGUI.PropertyField(rects[t++], property.FindPropertyRelative("costIncrement"));
                break;
            default:
				break;
		}

		if(EditorGUI.EndChangeCheck())
			property.serializedObject.ApplyModifiedProperties();

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

		m_numberOfFields =4;

		//+x for the spacing between the camps, 2 for each camp
		return EditorGUIUtility.singleLineHeight * m_numberOfFields + ((m_numberOfFields - 1) * 2);
	}
}
