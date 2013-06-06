/// <summary>
/// Dumps some simple terminal prefabs about the place.
/// 
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

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
		List<Room> availableRooms = new List<Room>();
		foreach(var room in m_level.Rooms)
		{
			if(!room.hasTerminal)
			{
				availableRooms.Add(room);	
			}
		}
		
		if(availableRooms.Count > 0)
		{
		
			LevelObjectDatabase database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
			
			LevelObject terminalObject = database.GetObject("terminal");
			int roomIndex = Random.Range(0, availableRooms.Count - 1);
			Room randomRoom = availableRooms[roomIndex];
			randomRoom.hasTerminal = true;
			
			terminalObject.Position = new Vector3(randomRoom.endX - 0.5f, randomRoom.startY + 0.5f, 0.0f);
			m_level.AddLevelObject(terminalObject);
		}
		
		m_placementAttempts++;
		
	}
	
	public void UpdateAll()
	{
		while(!StageComplete())
		{
			UpdateStep();
		}
	}
	
	public bool StageComplete() { return m_placementAttempts >= m_terminalTargetCount; }
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
	private int m_terminalTargetCount = 50;
	private int m_placementAttempts = 0;
}
