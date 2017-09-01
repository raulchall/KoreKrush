using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PathAgent))]
public class PathAgentCustomInspector : Editor
{
	PathAgent agent;
	ReorderableList editorList;

	void OnEnable ()
	{
		agent = target as PathAgent;
		CreateReordenableList ();
	}

	void CreateReordenableList()
	{
		//instance
		editorList = new ReorderableList (serializedObject, serializedObject.FindProperty ("additionalList"), true, true, true, true);
		//headerCallback
		editorList.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField (
				new Rect (rect.x, rect.y, 75, rect.height), "PositionInPath");
			EditorGUI.LabelField (
				new Rect (rect.x + rect.width-135, rect.y, 135, rect.height), "Gameobject");
			
			EditorGUI.LabelField (
				new Rect (rect.x + rect.width - 45, rect.y, 45, rect.height), "On/Off");
		};
		//elementDrawer
		editorList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = editorList.serializedProperty.GetArrayElementAtIndex (index);
			rect.y += 2;
			//float width = 60;
			//float rest = rect.width - width;

			EditorGUI.PropertyField (
				new Rect (rect.x, rect.y, 75, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative ("pathPosition"), GUIContent.none);
			EditorGUI.PropertyField (
				new Rect (rect.x+80, rect.y, 135, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative ("obj"), GUIContent.none);
			EditorGUI.PropertyField (
				new Rect (rect.x + rect.width-15, rect.y, 15, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("setEnableTo"), GUIContent.none);
		};
		//add_AddButtons

		//Add_RemoveButtons
		/*reordList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
			if(){
				
			}	
		};*/
	}

	public override void OnInspectorGUI()
	{
		var m_target = (PathAgent)target;
		DrawDefaultInspector ();
		editorList.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
		GUILayout.Space (5);
	}
}
