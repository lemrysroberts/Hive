/// <summary>
/// Dumps some simple terminal prefabs about the place.
/// 
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class PlaceTerminalsStage : IGeneratorStage
{
	public PlaceTerminalsStage(Level level)
	{
		m_level = level;	
	}
	
	public void Start(){ }
	public void End(){ }
	
	public void UpdateStep()
	{
		LevelObjectDatabase database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		
		LevelObject terminalObject = database.GetObject("terminal");
		Room randomRoom = m_level.Rooms[0];
		
		terminalObject.Position = new Vector3(randomRoom.endX - 0.5f, randomRoom.startY + 0.5f, 0.0f);
		m_level.AddLevelObject(terminalObject);
		
	}
	
	public void UpdateAll()
	{
		UpdateStep();
	}
	
	public bool StageComplete() { return true; }
	public void SetupGUI()
	{
#if UNITY_EDITOR
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, "Place Terminals");
		
		if(m_showFoldout)
		{
			
		}
#endif
	}
	
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Place Terminals"; }
	
	private Level m_level;
	private static bool m_showFoldout = false;
}
