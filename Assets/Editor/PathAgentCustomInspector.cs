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
		//instance                                                                                          draggable//header, 
		editorList = new ReorderableList (serializedObject, serializedObject.FindProperty ("PathSections"), true, true, true, true);


		//headerCallback
		editorList.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField (
				new Rect (rect.x, rect.y, 75, rect.height), "Section Starting Position");
			EditorGUI.LabelField (
				new Rect (rect.x + rect.width-135, rect.y, 135, rect.height), "Camera in this Section");
		};


		//elementDrawer
		editorList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = editorList.serializedProperty.GetArrayElementAtIndex (index);
			rect.y += 2;

			EditorGUI.PropertyField (
				new Rect (rect.x, rect.y, rect.width-80, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative ("CameraObj"), GUIContent.none);
			
			if (index != 0) {
				EditorGUI.PropertyField (
					new Rect (rect.width-40, rect.y, 75, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative ("PathPosition"), GUIContent.none);
			} else {
				EditorGUI.LabelField (
					new Rect (rect.width-40, rect.y, 75, rect.height), "0 (Fixed)");
			}
		};
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
