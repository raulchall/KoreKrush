using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using KoreKrush;
public class LevelEditor : EditorWindow {

	public Level level;

	static void Init()
	{
		EditorWindow.GetWindow (typeof(LevelEditor));
	}


}
