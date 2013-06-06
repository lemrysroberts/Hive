using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public void OnGUI()
	{
		GUILayout.BeginArea(new Rect((Screen.width / 2.0f) - 150.0f, 50, 300.0f, 500.0f));
		
		string[] gameModes = Enum.GetNames(typeof(WorldView));
		
		// Trim the "None" entry off the end.
		string[] trimmed = new string[gameModes.Length - 1];
		Array.Copy(gameModes, trimmed, gameModes.Length - 1);
		
		m_gameMode = GUILayout.SelectionGrid(m_gameMode, trimmed, trimmed.Length);
		
		GUILayout.BeginVertical(new GUIStyle("Box"));
		
		m_useCustomSeed = GUILayout.Toggle(m_useCustomSeed, "Use Custom Seed");
		
		GUI.enabled = m_useCustomSeed;
		
		m_customSeed = GUILayout.TextField(m_customSeed);
		
		GUI.enabled = true;
		
		int seed = 0;
		
		bool seedValid = int.TryParse(m_customSeed, out seed);
		
		seedValid |= !m_useCustomSeed;
		
		if(!seedValid)
		{
			Color oldColor = GUI.contentColor;
			GUI.contentColor = Color.red;
			
			GUILayout.Label("Invalid Seed");	
			
			GUI.contentColor = oldColor;
		}
		
		GUILayout.EndVertical();
		
		
		GUI.enabled = seedValid ;
		
		if(GUILayout.Button("Begin"))
		{
			GameFlow.Instance.View = (WorldView)m_gameMode;
			
			if(m_useCustomSeed && seedValid)
			{
				GameFlow.Instance.CustomSeed = seed;		
			}
			
			Application.LoadLevel("Hive");	
		}
		
		GUI.enabled = true;
		
		GUILayout.EndArea();
	}
	
	private bool m_useCustomSeed 	= false;
	private string m_customSeed 	= "0";
	private int m_gameMode 			= 0;
}
