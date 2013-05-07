using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelObject))] 
public class LevelObjectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		LevelObject levelObject = (LevelObject)target;
		GUILayout.Label("Current Prefab: " + levelObject.PrefabName);
		
		GameObject prefabObject = EditorGUILayout.ObjectField(null, typeof(GameObject), false) as GameObject;
		levelObject.SerialiseTarget = EditorGUILayout.ObjectField(levelObject.SerialiseTarget, typeof(SaveSerialisable), true) as SaveSerialisable;
		
		if(prefabObject != null)
		{
			GameObject prefab = PrefabUtility.FindPrefabRoot(prefabObject) as GameObject;	
			levelObject.PrefabName = prefab.name;
		}
	}
}

