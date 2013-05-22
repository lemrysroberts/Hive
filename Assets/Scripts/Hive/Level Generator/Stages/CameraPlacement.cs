#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class CameraPlacement : IGeneratorStage
{
	private Level m_level;
	private LevelObjectDatabase m_database;
	private int m_roomIndex;
	private int m_cameraChance = 25;
	private static bool m_showFoldout = false;
	
	public CameraPlacement(Level level)
	{
		m_level = level;
	}

	public void Start()
	{
		m_database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		m_roomIndex = 0;
	}
	
	public void End(){}
	
	public void UpdateStep()
	{
		if(!StageComplete())
		{
			AddCamera();
		}
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
			//The chance a room has a camera in it
			m_cameraChance = EditorGUILayout.IntSlider("Room Camera Chance", m_cameraChance, 1, 100);
		}
#endif
	}
	public void UpdateGUI() { }
	public void UpdateSceneGUI() { }
	
	
	public string GetStageName() { return "Camera Placement"; }
	
	public void AddCamera()
	{
		//@todo Need to check that
		
		Room currentRoom = m_level.Rooms[m_roomIndex];
		//Dice roll to decide if a room should have a camera
		if(Random.Range(0, 100) < m_cameraChance)
		{
			//Sweet, dice roll to pick a corner of the room. Index starts
			//bottom left and goes clockwise. Because.
			float cameraX = 0;
			float cameraY = 0;
			Quaternion orientation =  new Quaternion(0,0,0,0);
			
			//@todo Adjust for the size of the camera itself so we don't intersect the walls
			//@todo Make the arrow point
			switch(Random.Range(0, 4))
			{
			case 0:
				cameraX = currentRoom.startX;
				cameraY = currentRoom.startY;
				orientation = Quaternion.Euler(0,0,45);
				break;
			case 1:
				cameraX = currentRoom.startX;
				cameraY = currentRoom.endY;
				orientation = Quaternion.Euler(0,0,135);
				break;
			case 2:
				cameraX = currentRoom.endX;
				cameraY = currentRoom.endY;
				orientation = Quaternion.Euler(0,0,-135);
				break;
			case 3:
				cameraX = currentRoom.endX;
				cameraY = currentRoom.startY;
				orientation = Quaternion.Euler(0,0,-45);
				break;
			}
			
			LevelObject camera = m_database.GetObject("camera");
			if(camera != null)
			{
				camera.Position = new Vector3(cameraX, cameraY, -0.1f);
				camera.Rotation = orientation;
				m_level.AddLevelObject(camera);
			}
		}
		m_roomIndex++;
	}

}
