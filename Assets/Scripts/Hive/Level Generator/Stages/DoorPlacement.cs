#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class DoorPlacement : IGeneratorStage
{
	public DoorPlacement(Level level)
	{
		m_level = level;
	}
	
	public void Start() 
	{
		m_level.ClearObjects();	
	}
	
	public void End() {}
	
	public void UpdateStep() 
	{
		const int batchSize = 40;
		for(int i = 0; i < batchSize && !StageComplete(); i++)
		{
			AddDoor();
		}
	}
	
	public void UpdateAll() 
	{
		while(!StageComplete())
		{
			AddDoor();
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
			m_linkAllRooms = GUILayout.Toggle(m_linkAllRooms, "Link all neighbours");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Door Prefab", GUILayout.Width(90));
			m_level.DoorPrefab = EditorGUILayout.ObjectField(m_level.DoorPrefab, typeof(GameObject), false) as GameObject;
			m_level.AdminDoorPrefab = EditorGUILayout.ObjectField(m_level.AdminDoorPrefab, typeof(GameObject), false) as GameObject;
			
			GUILayout.EndHorizontal();
		}
#endif
	}
	public void UpdateGUI() { }
	public void UpdateSceneGUI()  
	{
#if UNITY_EDITOR
		foreach(var area in m_level.Corridors)
		{
			Handles.color = Color.green;
			Handles.DrawLine(new Vector2(area.startX, area.startY), new Vector2(area.endX, area.startY));
			Handles.DrawLine(new Vector2(area.startX, area.startY), new Vector2(area.startX, area.endY));
			Handles.DrawLine(new Vector2(area.startX, area.endY), new Vector2(area.endX, area.endY));
			Handles.DrawLine(new Vector2(area.endX, area.startY), new Vector2(area.endX, area.endY));	
		}
		
		foreach(var area in m_level.Rooms)
		{
			Handles.color = Color.red;
			Handles.DrawLine(new Vector2(area.startX, area.startY), new Vector2(area.endX, area.startY));
			Handles.DrawLine(new Vector2(area.startX, area.startY), new Vector2(area.startX, area.endY));
			Handles.DrawLine(new Vector2(area.startX, area.endY), new Vector2(area.endX, area.endY));
			Handles.DrawLine(new Vector2(area.endX, area.startY), new Vector2(area.endX, area.endY));
		}
		
		if(m_lastRoomA != null && m_lastRoomB != null)
		{
			Handles.color = Color.cyan;
			Vector3 pos1 = new Vector3(m_lastRoomA.startX + (m_lastRoomA.endX - m_lastRoomA.startX) / 2, m_lastRoomA.startY + (m_lastRoomA.endY - m_lastRoomA.startY) / 2, 1.0f);
			Vector3 pos2 = new Vector3(m_lastRoomB.startX + (m_lastRoomB.endX - m_lastRoomB.startX) / 2, m_lastRoomB.startY + (m_lastRoomB.endY - m_lastRoomB.startY) / 2, 1.0f);
			
			Handles.DrawLine(pos1, pos2);
		}	
#endif
	}
	
	public string GetStageName() { return "Door Placement"; }
	
	private void AddDoor()
	{
		Room currentRoom = m_level.Rooms[m_roomIndex];
		List<Corridor> corridors = GetAdjacentCorridors(currentRoom);
		
		if(corridors.Count > 0)
		{
			Corridor corridor = corridors[Random.Range(0, corridors.Count)];
			
			if(corridor.endX < currentRoom.startX)
			{
				int minY = Mathf.Max(corridor.startY, currentRoom.startY);
				int maxY = Mathf.Min(corridor.endY, currentRoom.endY);
				int doorY = Random.Range(minY, maxY);
				m_level.SetTileID(corridor.endX, doorY, 1, false);
				if(m_level.DoorPrefab != null)
				{
					DoorObject newDoor = new DoorObject();
					newDoor.AgentPrefab = m_level.DoorPrefab;
					newDoor.AdminPrefab = m_level.AdminDoorPrefab;
					newDoor.Position = new Vector3(corridor.endX + 0.5f, doorY + 0.5f, 0.0f);
					newDoor.Rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
					m_level.AddGameObject(newDoor);
				}
			}
			
			if(corridor.startX >= currentRoom.endX)
			{
				int minY = Mathf.Max(corridor.startY, currentRoom.startY);
				int maxY = Mathf.Min(corridor.endY, currentRoom.endY);
				int doorY = Random.Range(minY, maxY);
				m_level.SetTileID(corridor.startX - 1, doorY, 1, false);
				
				if(m_level.DoorPrefab != null)
				{
					DoorObject newDoor = new DoorObject();
					newDoor.AgentPrefab = m_level.DoorPrefab;
					newDoor.AdminPrefab = m_level.AdminDoorPrefab;
					newDoor.Position = new Vector3(corridor.startX - 0.5f, doorY + 0.5f, 0.0f);
					newDoor.Rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
					m_level.AddGameObject(newDoor);
				}
			}
				
			if(corridor.startY >= currentRoom.endY)
			{
				int minX = Mathf.Max(corridor.startX, currentRoom.startX);
				int maxX = Mathf.Min(corridor.endX, currentRoom.endX);
				int doorX = Random.Range(minX, maxX);
				m_level.SetTileID(doorX, corridor.startY - 1, 1, false);
				
				if(m_level.DoorPrefab != null)
				{
					DoorObject newDoor = new DoorObject();
					newDoor.AgentPrefab = m_level.DoorPrefab;
					newDoor.AdminPrefab = m_level.AdminDoorPrefab;
					newDoor.Position = new Vector3(doorX + 0.5f, corridor.startY - 0.5f, 0.0f);
					m_level.AddGameObject(newDoor);
				}
			}
				
			if(corridor.endY < currentRoom.startY)
			{
				int minX = Mathf.Max(corridor.startX, currentRoom.startX);
				int maxX = Mathf.Min(corridor.endX, currentRoom.endX);
				int doorX = Random.Range(minX, maxX);
				m_level.SetTileID(doorX, corridor.endY, 1, false);
				if(m_level.DoorPrefab != null)
				{
					DoorObject newDoor = new DoorObject();
					newDoor.AgentPrefab = m_level.DoorPrefab;
					newDoor.AdminPrefab = m_level.AdminDoorPrefab;
					newDoor.Position = new Vector3(doorX + 0.5f, corridor.endY + 0.5f, 0.0f);
					m_level.AddGameObject(newDoor);
				}
			}
		}
		else
		{
			// Internal room - Find an adjacent room.
			// This can be simplified with more spatial data. Just like everything else.
			foreach(var other in m_level.Rooms)
			{
				if(other == currentRoom || other.ConnectedRooms.Contains(currentRoom) || currentRoom.ConnectedRooms.Contains(other))
				{
					continue;	
				}
				
				if( other.endX < currentRoom.startX - 1 ||
					other.startX > currentRoom.endX + 1 ||
					other.endY < currentRoom.startY - 1 ||
					other.startY > currentRoom.endY + 1)
				{
					continue;	
				}
				
				bool linkMade = false;
				if(other.endX < currentRoom.startX && (other.startY < currentRoom.endY && other.endY > currentRoom.startY))
				{
					int minY = Mathf.Max(other.startY, currentRoom.startY);
					int maxY = Mathf.Min(other.endY, currentRoom.endY);
					int doorY = Random.Range(minY, maxY);
					m_level.SetTileID(other.endX, doorY, 3, false);
					linkMade = true;
					
					if(m_level.DoorPrefab != null)
					{
						DoorObject newDoor = new DoorObject();
						newDoor.AgentPrefab = m_level.DoorPrefab;
						newDoor.AdminPrefab = m_level.AdminDoorPrefab;
						newDoor.Position = new Vector3(other.endX + 0.5f, doorY + 0.5f, 0.0f);
						newDoor.Rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
						m_level.AddGameObject(newDoor);
					}
				}
				
				if(other.startX >= currentRoom.endX && (other.startY < currentRoom.endY && other.endY > currentRoom.startY))
				{
					int minY = Mathf.Max(other.startY, currentRoom.startY);
					int maxY = Mathf.Min(other.endY, currentRoom.endY);
					int doorY = Random.Range(minY, maxY);
					m_level.SetTileID(other.startX - 1, doorY, 3, false);
					linkMade = true;
					
					if(m_level.DoorPrefab != null)
					{
						DoorObject newDoor = new DoorObject();
						newDoor.AgentPrefab = m_level.DoorPrefab;
						newDoor.AdminPrefab = m_level.AdminDoorPrefab;
						newDoor.Position = new Vector3(other.startX - 0.5f, doorY + 0.5f, 0.0f);
						newDoor.Rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
						m_level.AddGameObject(newDoor);
					}
				}
					
				if(other.startY >= currentRoom.endY && (other.startX < currentRoom.endX && other.endX > currentRoom.startX))
				{
					int minX = Mathf.Max(other.startX, currentRoom.startX);
					int maxX = Mathf.Min(other.endX, currentRoom.endX);
					int doorX = Random.Range(minX, maxX);
					m_level.SetTileID(doorX, other.startY - 1, 3, false);
					linkMade = true;
					
					if(m_level.DoorPrefab != null)
					{
						DoorObject newDoor = new DoorObject();
						newDoor.AgentPrefab = m_level.DoorPrefab;
						newDoor.AdminPrefab = m_level.AdminDoorPrefab;
						newDoor.Position = new Vector3(doorX + 0.5f, other.startY - 0.5f, 0.0f);
						newDoor.Rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
						m_level.AddGameObject(newDoor);
					}
				}
					
				if(other.endY < currentRoom.startY && (other.startX < currentRoom.endX && other.endX > currentRoom.startX))
				{
					int minX = Mathf.Max(other.startX, currentRoom.startX);
					int maxX = Mathf.Min(other.endX, currentRoom.endX);
					int doorX = Random.Range(minX, maxX);
					m_level.SetTileID(doorX, other.endY, 3, false);
					linkMade = true;
					
					if(m_level.DoorPrefab != null)
					{
						DoorObject newDoor = new DoorObject();
						newDoor.AgentPrefab = m_level.DoorPrefab;
						newDoor.AdminPrefab = m_level.AdminDoorPrefab;
						newDoor.Position = new Vector3(doorX + 0.5f, other.endY + 0.5f, 0.0f);
						newDoor.Rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
						m_level.AddGameObject(newDoor);
					}
				}
				
				if(linkMade)
				{
					other.ConnectedRooms.Add(currentRoom);
					currentRoom.ConnectedRooms.Add(other);
					m_lastRoomA = currentRoom;
					m_lastRoomB = other;
					if(!m_linkAllRooms)
					{
						break;
					}
				}
			}
		}
		
		m_roomIndex++;	
	}
	
	public List<Corridor> GetAdjacentCorridors(Room room)
	{
		List<Corridor> adjacentCorridors = new List<Corridor>();
		
		foreach(var corridor in m_level.Corridors)
		{
			
			// Above	
			if(corridor.startY == room.endY + 1 && corridor.startX <= room.startX && corridor.endX >= room.endX)
			{
				adjacentCorridors.Add(corridor);	
			}
			
			// Below
			if(corridor.endY == room.startY - 1 && corridor.startX <= room.startX && corridor.endX >= room.endX)
			{
				adjacentCorridors.Add(corridor);	
			}
			
			// Right
			if(corridor.startX == room.endX + 1 && corridor.startY <= room.startY && corridor.endY >= room.endY)
			{
				adjacentCorridors.Add(corridor);	
			}
			
			// Left
			if(corridor.endX == room.startX - 1 && corridor.startY <= room.startY && corridor.endY >= room.endY)
			{
				adjacentCorridors.Add(corridor);	
			}
			
		}
		
		return adjacentCorridors;
	}
	
	private Room m_lastRoomA = null;
	private Room m_lastRoomB = null;
	
	private Level m_level;
	private int m_roomIndex = 0;
	private static bool m_showFoldout = false;	
	private static bool m_linkAllRooms = true;
}
