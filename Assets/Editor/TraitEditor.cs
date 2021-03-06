using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Trait)), CanEditMultipleObjects]
public class TraitEditor : Editor
{
	private ReorderableList reorderableTestsList;
	private float lineHeightSpace;
	private int[] testNumber = new int[4];

	void OnEnable() {

		lineHeightSpace = EditorGUIUtility.singleLineHeight + 3;

		reorderableTestsList = new ReorderableList(serializedObject, serializedObject.FindProperty("EQSTests"), true, true, true, true) {
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Tests");
			}
		};

		reorderableTestsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			SerializedProperty element = reorderableTestsList.serializedProperty.GetArrayElementAtIndex(index);

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

		reorderableTestsList.elementHeightCallback = (int index) => {
			float height = 0;

			SerializedProperty element = reorderableTestsList.serializedProperty.GetArrayElementAtIndex(index);

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

		reorderableTestsList.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Distance"), false, AddEQSTestHandler, "Distance");
			menu.AddItem(new GUIContent("Angle"), false, AddEQSTestHandler, "Angle");
			menu.AddItem(new GUIContent("Raycast"), false, AddEQSTestHandler, "Raycast");
			menu.AddItem(new GUIContent("Tag"), false, AddEQSTestHandler, "Tag");
			menu.DropDown(buttonRect);
		};

		reorderableTestsList.onRemoveCallback = (ReorderableList list) => {
			var temp= reorderableTestsList.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue;

			if(temp.GetType().Equals(typeof(TestDistance)))
				testNumber[0]--;
			else if(temp.GetType().Equals(typeof(TestAngle)))
				testNumber[1]--;
			else if(temp.GetType().Equals(typeof(TestRaycast)))
				testNumber[2]--;
			else if(temp.GetType().Equals(typeof(TestTag)))
				testNumber[3]--;

			DestroyImmediate(reorderableTestsList.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue, true);

			AssetDatabase.SaveAssets();

			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
		};
	}

	private void AddEQSTestHandler(object type) {
		string testType = (string)type;
		if(testType == "Distance") {
			int index = reorderableTestsList.serializedProperty.arraySize;
			reorderableTestsList.serializedProperty.arraySize++;
			reorderableTestsList.index = index;

			SerializedProperty element = reorderableTestsList.serializedProperty.GetArrayElementAtIndex(index);
			TestDistance t = CreateInstance<TestDistance>();
			t.name = target.name + ": Distance Test " + (++testNumber[0]);
			AssetDatabase.AddObjectToAsset(t, target);
			AssetDatabase.SaveAssets();
			element.objectReferenceValue = t;
		}
		else if(testType == "Angle") {
			int index = reorderableTestsList.serializedProperty.arraySize;
			reorderableTestsList.serializedProperty.arraySize++;
			reorderableTestsList.index = index;

			SerializedProperty element = reorderableTestsList.serializedProperty.GetArrayElementAtIndex(index);
			TestAngle t = CreateInstance<TestAngle>();
			t.name = target.name + ": Angle Test " + (++testNumber[1]);
			AssetDatabase.AddObjectToAsset(t, target);
			AssetDatabase.SaveAssets();
			element.objectReferenceValue = t;
		}
		else if(testType == "Raycast") {
			int index = reorderableTestsList.serializedProperty.arraySize;
			reorderableTestsList.serializedProperty.arraySize++;
			reorderableTestsList.index = index;

			SerializedProperty element = reorderableTestsList.serializedProperty.GetArrayElementAtIndex(index);
			TestRaycast t = CreateInstance<TestRaycast>();
			t.name = target.name + ": Raycast Test " + (++testNumber[2]);
			AssetDatabase.AddObjectToAsset(t, target);
			AssetDatabase.SaveAssets();
			element.objectReferenceValue = t;
		}
		else if(testType == "Tag") {
			int index = reorderableTestsList.serializedProperty.arraySize;
			reorderableTestsList.serializedProperty.arraySize++;
			reorderableTestsList.index = index;

			SerializedProperty element = reorderableTestsList.serializedProperty.GetArrayElementAtIndex(index);
			TestTag t = CreateInstance<TestTag>();
			t.name = target.name + ": Tag Test " + (++testNumber[3]);
			AssetDatabase.AddObjectToAsset(t, target);
			AssetDatabase.SaveAssets();
			element.objectReferenceValue = t;
		}
		serializedObject.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		DrawDefaultInspector();
		reorderableTestsList.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}
}
