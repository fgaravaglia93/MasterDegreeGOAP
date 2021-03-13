using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PersonalityAgent)), CanEditMultipleObjects, ExecuteInEditMode]
public class AgentEditor : Editor
{
	private ReorderableList reorderableTestList;
	private float lineHeightSpace;
	Agent agent;

	void OnEnable() {

		agent= (Agent)target;
		lineHeightSpace = EditorGUIUtility.singleLineHeight + 3;

		reorderableTestList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_traits"), true, true, false, false) {
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "EQS Traits");
			}
		};

		reorderableTestList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			SerializedProperty element = reorderableTestList.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.displayName);

			if(element.objectReferenceValue != null) {
				SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
				SerializedProperty propertyIterator = elementObj.GetIterator();

				int i = 1;
				while(propertyIterator.NextVisible(true)) {
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, EditorGUIUtility.singleLineHeight), propertyIterator);
					i++;
				}

				elementObj.ApplyModifiedProperties();
			}
		};

        reorderableTestList.elementHeightCallback = (int index) => {
			float height = 0;

            SerializedProperty element = reorderableTestList.serializedProperty.GetArrayElementAtIndex(index);

			int i = 1;
			if(element.objectReferenceValue != null) {
				SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
				SerializedProperty propertyIterator = elementObj.GetIterator();

				while(propertyIterator.NextVisible(true)) {
					i++;
				}
			}

			height = lineHeightSpace * i;

			return height;
		};
	}

	public override void OnInspectorGUI() {
        DrawDefaultInspector();
        serializedObject.Update();
        //reorderableTestList.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
		if(GUI.changed) {
			agent.RecalculateTraitList();
			agent.RecalculateTestsRuntimeVariables();
		}
		//agent.RecalculateTraitList();
		if(GUILayout.Button("RecalculateTestsRuntimeVariables")) {
			agent.RecalculateTestsRuntimeVariables();
		}
	}
}
