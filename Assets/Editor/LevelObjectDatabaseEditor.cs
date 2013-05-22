using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelObjectDatabaseEditor : EditorWindow
{
	/// <summary>
	/// Shows the editor window.
	/// </summary>
	[MenuItem("Optimism/Level-Object Database")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(LevelObjectDatabaseEditor));
	}
	
	void OnEnable()
	{
		m_errorStyle.normal.textColor = Color.red; 
		
	}
	
	void OnGUI()
	{
		m_database = FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		
		if(m_database == null)
		{
			GUILayout.Label("No Database Found. Ensure a gameObject exists with the \"LevelObjectDatabase\" component.");
			return;
		}
		
		GUILayout.BeginArea(new Rect(0, 3, position.width, position.height - m_statusBarHeight - 10));
		
		GUILayout.BeginHorizontal();
		
		DrawList(m_database);
		
		if(m_activeObject != null)
		{
			if(DrawLevelObject(m_activeObject))
			{
				m_database.LevelObjects.Remove(m_activeObject);
				m_activeObject = null;
			}
		}

		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		
		GUI.Box(new Rect(0, position.height - m_statusBarHeight, position.width, 1), "");
		GUI.Label(new Rect(3, position.height - m_statusBarHeight + 3, position.width, m_statusBarHeight), m_lastError, m_errorStyle);
	}
	
	private void DrawList(LevelObjectDatabase database)
	{
		GUILayout.BeginVertical();
		
		GUI.Box(new Rect(0, 3, 200, position.height - m_statusBarHeight - 13 - m_addBoxHeight), "");
		m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(200), GUILayout.Height(position.height - m_statusBarHeight  - 13 - m_addBoxHeight));
		
		foreach(var levelObject in m_database.LevelObjects)
		{
			GUILayout.Label(levelObject.Name);
			
			if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
			{
				Repaint();
				if( Event.current.type == EventType.mouseDown)
				{
					m_activeObject = levelObject;
				}
			}
		}
		
		GUILayout.EndScrollView();
		
			
		GUILayout.BeginArea(new Rect(3, position.height - m_statusBarHeight - m_addBoxHeight, 190, m_addBoxHeight));
			
		GUILayout.BeginHorizontal();
		m_newObjectName = GUILayout.TextField(m_newObjectName, GUILayout.Width(150));
		
		if(m_newObjectName == string.Empty) 
		{
			GUI.enabled = false;	
		}
		
		if(GUILayout.Button("Add"))
		{
			LevelObject newObject = m_database.AddObject(m_newObjectName);
			
			m_lastError = string.Empty;
			if(newObject == null) 
			{
				m_lastError = m_database.LastError;	
			}
		}
		GUI.enabled = true;
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		
		GUILayout.EndVertical();
		
	}
	
	private bool DrawLevelObject(LevelObject levelObject)
	{
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Name", GUILayout.Width(150));
		string newName = GUILayout.TextField(levelObject.Name);
		GUILayout.EndHorizontal();
		
		bool duplicateFound = false;
		if(newName != levelObject.Name)
		{
			foreach(var currentObject in m_database.LevelObjects)
			{
				if(currentObject == levelObject)
				{
					continue;	
				}
				
				if(currentObject.Name.ToLower() == newName.ToLower())
				{
					duplicateFound = true;
					m_lastError = newName + " already exists.";
					break;
				}
			}
			if(!duplicateFound)
			{
				levelObject.Name = newName;	
				m_lastError = string.Empty;
			}
		}
		
		levelObject.AdminPrefab = EditorGUILayout.ObjectField("Administrator Prefab", levelObject.AdminPrefab, typeof(GameObject), false) as GameObject;
		levelObject.AgentPrefab = EditorGUILayout.ObjectField("Agent Prefab", levelObject.AgentPrefab, typeof(GameObject), false) as GameObject;
		
		GUILayout.BeginHorizontal();
		
		GUILayout.Label("Synchronisation Script ", GUILayout.Width(150));
		UnityEditor.MonoScript syncScript = EditorGUILayout.ObjectField(null, typeof(UnityEditor.MonoScript), false) as UnityEditor.MonoScript;
		
		if(syncScript != null)
		{
			if(syncScript.GetClass().FullName != levelObject.SynchronisationScript)
			{
				levelObject.SynchronisationScript = syncScript.GetClass().FullName;
			}
		} 
		
		GUILayout.EndHorizontal();
		GUILayout.Label("Current Script: " +  (levelObject.SynchronisationScript == null ? "Not Set" : levelObject.SynchronisationScript));
		
		bool delete = false;
		
		if(GUILayout.Button("Delete Object"))
		{
			if(EditorUtility.DisplayDialog("Delete " + levelObject.Name + "?", "This will permanently delete " + levelObject.Name + ". Continue?", "Ok", "Cancel"))
			{
				delete = true;		
			}
		}
		
		GUILayout.EndVertical();
		
		return delete;
	}
	
	private Vector2 m_scrollPos	= Vector2.zero;
	private string m_lastError 	= string.Empty; 
	private GUIStyle m_errorStyle = new GUIStyle(); 
	private const int m_statusBarHeight = 20;
	private const int m_addBoxHeight = 30;
	private string m_newObjectName = string.Empty;
	private LevelObjectDatabase m_database = null;
	private LevelObject m_activeObject = null;
}
