#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class RoomBuilder : IGeneratorStage
{
	public RoomBuilder(Level level)
	{
		m_level = level;
	}
	
	public void Start() 
	{
		m_level.Rooms.Clear();
		m_corridorPartitions = m_level.RoomAreas;
		
		if(m_corridorPartitions == null)
		{
			Debug.LogError("Corridor partition data missing");	
		}
	}
	
	public void End() {}
	
	public void UpdateStep() 
	{
		ProcessArea();
	}
	
	public void UpdateAll() 
	{
		while(m_currentAreaIndex < m_corridorPartitions.Count)
		{
			ProcessArea();
		}
	}
	
	public bool StageComplete() 
	{ 
		return m_currentAreaIndex == m_corridorPartitions.Count;
	}
	
	public void SetupGUI()
	{
#if UNITY_EDITOR
		
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, GetStageName());
		if(m_showFoldout)
		{
			m_minX 			= EditorGUILayout.IntField("Area Minimum X", m_minX);	
			m_minY 			= EditorGUILayout.IntField("Area Minimum Y", m_minY);	
			
			m_minX 			= Mathf.Max(m_minX, 1);
			m_minY 			= Mathf.Max(m_minY, 1);
		}
#endif
	}
	
	private void ProcessArea()
	{
		AreaPartition area = m_corridorPartitions[m_currentAreaIndex];
		

		m_activeAreas.Clear();
		
		area.sliceType = AreaPartition.SliceType.Vertical;
		area.failCount = 0;
		area.startX += 1;
		area.startY += 1;
		area.endX -= 1;
		area.endY -=1;
		m_activeAreas.Add(area);
		
		
		m_roomsGenerated = 0;
		m_splitPossible = true;
		m_sliceHorizontal = true;
		
		while(m_roomsGenerated < m_maxRooms && m_splitPossible) 
		{
			AreaPartition topArea = GetLargestArea(m_sliceHorizontal ? AreaPartition.SliceType.Vertical : AreaPartition.SliceType.Horizontal);
			if(topArea == null)
			{
				m_lastError = "No Top Area";
				m_splitPossible = false;
				continue;
			}
			
			int startValue 	= m_sliceHorizontal ? topArea.startY : topArea.startX;
			int endValue	= m_sliceHorizontal ? topArea.endY : topArea.endX;
			
			int extents = endValue - startValue;
			
			// Determine the amount of space used by the walls and corridor, as these must be accounted for when checking for enough space.
			int usedSpace = m_wallWidth;
			usedSpace += 4; 											// Wall widths. This needs to be changed if wall thickness becomes adjustable.
			usedSpace += m_sliceHorizontal ? m_minY * 2 : m_minX * 2; 	// Account for the two new areas.
			int range = extents - usedSpace;
			
			// If there's insufficient room, flip the slice value and quit out for this run.
			if( (m_sliceHorizontal && range < m_minY) || (!m_sliceHorizontal && range < m_minX))
			{
				topArea.failCount++;
				m_lastError = (m_sliceHorizontal ? "Horizontal " : "Vertical") + "Region too small";
				m_sliceHorizontal = !m_sliceHorizontal;
				topArea.sliceType = topArea.sliceType == AreaPartition.SliceType.Horizontal ? AreaPartition.SliceType.Vertical : AreaPartition.SliceType.Horizontal;
				m_roomsGenerated++;
				continue;
			}
			
			// Determine a random split-location.
			int split = Random.Range(startValue + usedSpace / 2, endValue - usedSpace / 2);
			
			// Corridor			
			Room newRoom = new Room();
			newRoom.startX 	= m_sliceHorizontal ? topArea.startX : split - m_wallWidth / 2;
			newRoom.endX	= m_sliceHorizontal ? topArea.endX : split;
			newRoom.startY 	= m_sliceHorizontal ? split - m_wallWidth / 2 : topArea.startY;
			newRoom.endY 	= m_sliceHorizontal ? split: topArea.endY;
			
			//m_level.Rooms.Add(newRoom);
			
			// New areas
			AreaPartition area1 = new AreaPartition();
			AreaPartition area2 = new AreaPartition();
			
			area1.startX 	= topArea.startX;
			area1.endX 		= m_sliceHorizontal ? topArea.endX : split - m_wallWidth / 2;
			area1.startY 	= topArea.startY;
			area1.endY 		= m_sliceHorizontal ? split - m_wallWidth / 2 : topArea.endY;
			area1.sliceType = m_sliceHorizontal? AreaPartition.SliceType.Horizontal : AreaPartition.SliceType.Vertical;
			
			area2.startX 	= m_sliceHorizontal ? topArea.startX : split;
			area2.endX 		= topArea.endX;
			area2.startY 	= m_sliceHorizontal ? split: topArea.startY;
			area2.endY 		= topArea.endY;
			area2.sliceType = m_sliceHorizontal? AreaPartition.SliceType.Horizontal : AreaPartition.SliceType.Vertical;
			
			m_activeAreas.Add(area1);
			m_activeAreas.Add(area2);
			
			m_activeAreas.Remove(topArea);
			
			
			// Draw the corridor
			for(int x = newRoom.startX; x < newRoom.endX; x++)
			{
				for(int y = newRoom.startY; y < newRoom.endY; y++)
				{
					m_level.SetTileID(x, y, 0, false);
				}
			}
			
			m_roomsGenerated++;
			m_sliceHorizontal = !m_sliceHorizontal;
		}
		
		foreach(var roomArea in m_activeAreas)
		{
			Room newRoom = new Room();
			newRoom.startX = roomArea.startX ;
			newRoom.startY = roomArea.startY;
			newRoom.endX = roomArea.endX ;
			newRoom.endY = roomArea.endY;
			m_level.Rooms.Add(newRoom);
			
			for(int x = newRoom.startX; x < newRoom.endX; x++)
			{
				for(int y = newRoom.startY; y < newRoom.endY; y++)
				{
					m_level.SetTileID(x, y, 3, false);
				}
			}
		}
		
		
		m_currentAreaIndex++;
	}
	
	private void GrowRoom(Room room)
	{
		List<Vector2> tilePoints = new List<Vector2>();
		tilePoints = new List<Vector2>(room.tileIDs);
		
		// TODO: This will produce loads of duplicates
		foreach(var point in room.tileIDs)
		{
			Vector2 right 		= new Vector2(point.x + 1.0f, point.y);
			Vector2 left 		= new Vector2(point.x - 1.0f, point.y);
			Vector2 up 			= new Vector2(point.x, point.y + 1.0f);
			Vector2 down 		= new Vector2(point.x, point.y - 1.0f);
			Vector2 rightUp		= new Vector2(point.x + 1.0f, point.y + 1.0f);
			Vector2 rightDown 	= new Vector2(point.x + 1.0f, point.y + 1.0f);
			Vector2 leftUp		= new Vector2(point.x - 1.0f, point.y + 1.0f);
			Vector2 leftDown	= new Vector2(point.x - 1.0f, point.y - 1.0f);
			
			if(!m_level.TileBlocked((int)(right.x), (int)right.y)) 			{ tilePoints.Add(right); }
			if(!m_level.TileBlocked((int)(left.x), (int)left.y)) 			{ tilePoints.Add(left); }
			if(!m_level.TileBlocked((int)(up.x), (int)up.y)) 				{ tilePoints.Add(up); }
			if(!m_level.TileBlocked((int)(down.x), (int)down.y)) 			{ tilePoints.Add(down); }
			if(!m_level.TileBlocked((int)(rightUp.x), (int)rightUp.y)) 		{ tilePoints.Add(rightUp); }
			if(!m_level.TileBlocked((int)(rightDown.x), (int)rightDown.y)) 	{ tilePoints.Add(rightDown); }
			if(!m_level.TileBlocked((int)(leftUp.x), (int)leftUp.y)) 		{ tilePoints.Add(leftUp); }
			if(!m_level.TileBlocked((int)(leftDown.x), (int)leftDown.y)) 	{ tilePoints.Add(leftDown); }
			
		}
		
		room.tileIDs = new List<Vector2>(tilePoints);
	}
	
	public void UpdateGUI() 
	{
		GUILayout.Label("Sections processed: " 	+ m_currentAreaIndex + " / " + m_corridorPartitions.Count);
		GUILayout.Label("Last Error: " + m_lastError);
	}
	
	public void UpdateSceneGUI() 
	{
#if UNITY_EDITOR
		foreach(var area in m_activeAreas)
		{
			Handles.color = Color.green;
			Handles.DrawLine(new Vector2(area.startX, area.startY), new Vector2(area.endX, area.startY));
			Handles.DrawLine(new Vector2(area.startX, area.startY), new Vector2(area.startX, area.endY));
			Handles.DrawLine(new Vector2(area.startX, area.endY), new Vector2(area.endX, area.endY));
			Handles.DrawLine(new Vector2(area.endX, area.startY), new Vector2(area.endX, area.endY));	
		}
#endif
	}
	
	private AreaPartition GetLargestArea(AreaPartition.SliceType type)
	{
		AreaPartition currentLargest = null;
		int currentMetric 		= int.MinValue;
		
		foreach(var area in m_activeAreas)
		{
			if(area.sliceType != type || area.failCount == 2)	{ continue;	}
			
			int metric = area.GetMetric();
			if(metric > currentMetric)
			{
				currentLargest = area;
				currentMetric = metric;
			}
		}
		return currentLargest;
	}
	
	public string GetStageName() { return "Room Builder"; }
	
	private Level m_level = null;
	private List<AreaPartition> m_corridorPartitions = null;
	private List<AreaPartition> m_activeAreas = new List<AreaPartition>();
	private int m_currentAreaIndex = 0;
	private int m_roomsGenerated 	= 0;
	private bool m_sliceHorizontal 		= true;
	private static int m_maxRooms	= 60;
	private bool m_splitPossible 	= true;
	private static int m_wallWidth 	= 2;
	private static int m_minX 			= 1;
	private static int m_minY 			= 1;
	private string m_lastError		= string.Empty;
	
	private static bool m_showFoldout = false;
}
