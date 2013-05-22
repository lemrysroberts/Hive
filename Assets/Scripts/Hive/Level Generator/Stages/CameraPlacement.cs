#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class CameraPlacement : IGeneratorStage
{
	private Level m_level;
	private static bool m_showFoldout = false;
	private int m_roomIndex = 0;
	
	public CameraPlacement(Level level)
	{
		m_level = level;
	}
	
	public void Start()
	{
		m_roomIndex = 0;
	}
	
	public void End() {}
	
	public void UpdateStep() 
	{
		AddCamera();
	}
	
	public void UpdateAll()
	{
		while(!StageComplete())
		{
			AddCamera();
		}
	}
	
	public bool StageComplete()
	{
		return m_roomIndex == m_level.Rooms.Count;
	}
	
	public void SetupGUI() 
	{
#if UNITY_EDITOR
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, GetStageName());
		if(m_showFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Camera Prefab", GUILayout.Width(90));
			m_level.CameraPrefab = EditorGUILayout.ObjectField(m_level.CameraPrefab, typeof(GameObject), false) as GameObject;
			
			GUILayout.EndHorizontal();
		}
#endif
	}
	
	public void UpdateGUI() { }
	public void UpdateSceneGUI()  
	{
#if UNITY_EDITOR
		//Could shove camera view stuff here
#endif
	}
	
	public string GetStageName() { return "Camera Placement"; }
	
	private void AddCamera()
	{
		Room currentRoom = m_level.Rooms[m_roomIndex];
		
		// Room has 25% chance of having a camera in it
		if(Random.Range(0, 4) == 3)
		{
			Vector3 position = new Vector3(0,0,0);
			//Randomly pick the corner, indexed from bottom left going clockwise.  Because.
			switch(Random.Range(0,3))
			{
			case 0:
				position = new Vector3(currentRoom.startX, currentRoom.startY, -1.0f);
				break;
			case 1:
				position = new Vector3(currentRoom.startX, currentRoom.endY, -1.0f);
				break;
			case 2:
				position = new Vector3(currentRoom.endX, currentRoom.endY, -1.0f);
				break;
			case 3:
				position = new Vector3(currentRoom.endX, currentRoom.startY, -1.0f);
				break;
			}
			
			CameraObject securityCamera = new CameraObject();
			securityCamera.Prefab = m_level.CameraPrefab;
			securityCamera.Position = position;
			m_level.AddGameObject(securityCamera);			
		}
		
		m_roomIndex++;
	}
}
