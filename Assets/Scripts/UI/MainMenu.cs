using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	
	public void Start()
	{
	
	}
	
	public void OnGUI()
	{
		string dataPath = Application.dataPath;
		dataPath += "/levels";
		
		object[] levels =  Resources.LoadAll("Levels");
		string[] files = new string[levels.Length];
		
		int count = 0;
		foreach(object level in levels)
		{
			TextAsset text = level as TextAsset;
			
			if(text != null)
			{
				files[count] = text.name;
				count++;
			}
		}
		
		GUILayout.BeginArea(new Rect((Screen.width / 2.0f) - 150.0f, 50, 300.0f, 50.0f));
		
		string[] gameModes = Enum.GetNames(typeof(WorldView));
		m_gameMode = GUILayout.SelectionGrid(m_gameMode, gameModes, gameModes.Length);
		
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect((Screen.width / 2.0f) - 150.0f, 100, 300.0f, 200.0f));
		
		m_scrollProgress = GUILayout.BeginScrollView(m_scrollProgress);
		
		m_currentSelection = GUILayout.SelectionGrid(m_currentSelection, files, 1);
		
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect((Screen.width / 2.0f) - 150.0f, 300, 300.0f, 100.0f));
		
		if(GUILayout.Button("Begin"))
		{
			GameFlow.Instance.View = (WorldView)m_gameMode;
			GameFlow.Instance.CurrentLevel = "Levels/" +  files[m_currentSelection];
			Application.LoadLevel("Hive");	
		}
		
		GUILayout.EndArea();
		
	}
	
		
	private Vector2 m_scrollProgress = new Vector2();
	private int m_currentSelection = 0;
	private int m_gameMode = 0;
}
