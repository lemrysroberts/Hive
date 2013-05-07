#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


public class DebugRoomsStage : IGeneratorStage
{
	Level m_level = null;
	
	public DebugRoomsStage(Level level)
	{
		m_level = level;
	}
	
	public void Start()
	{
		m_targetCorridorCount = 100;
		m_roomCandidateIterationCount = 2000;
		m_targetRoomCount = 50;
		m_currentRoomCount = 0;
		m_levelWidth = m_level.SectionCountX * Level.m_sectionSize;
		m_levelHeight = m_level.SectionCountY * Level.m_sectionSize;
		
		//m_sectionHeap.Reset();
		m_splitPossible = true;
		
		RoomArea startArea = new RoomArea();
		startArea.startX 	= 0;
		startArea.startY 	= 0;
		startArea.endX		= m_levelWidth;
		startArea.endY		= m_levelHeight;
		startArea.m_type = RoomArea.SliceType.Vertical;
		
		m_activeAreas.Add(startArea);
		//m_sectionHeap.Insert(startArea, startArea.GetMetric());
		
	}
	
	public void End() {}
	
	public void UpdateStep()
	{
		GenerateRoom();
		m_level.RebuildAllSections();
	}
	
	public void UpdateAll()
	{
		for(int i = 0; i < m_targetRoomCount; i++)
		{
			GenerateRoom();
		}
		
		m_level.RebuildAllSections();
	}
	
	public bool StageComplete()
	{
		return !m_splitPossible || m_corridorsGenerated == m_targetCorridorCount;
		//return m_roomFailed || m_currentRoomCount == m_targetRoomCount;
	}
	
	public void UpdateGUI()
	{
#if UNITY_EDITOR
		//Debug.Log ("test");
		GUILayout.Label("Rooms Generated: " + m_currentRoomCount);
#endif
	}
	
	public void SetupGUI() {}
	
	public void UpdateSceneGUI()
	{
	}
	
	public string GetStageName()
	{
		return "Debug Rooms";	
	}
	
	private void GenerateRoom()
	{
		int minX = 10;
		int minY = 10;
		
		if(m_corridorsGenerated < m_targetCorridorCount && m_splitPossible)
		{
			RoomArea topArea = GetLargestArea(m_sliceHorizontal ? RoomArea.SliceType.Vertical : RoomArea.SliceType.Horizontal);
			m_activeAreas.Remove(topArea);
			
			if(topArea == null)
			{
				m_splitPossible = false;
				return;
			}
						
			if(!m_sliceHorizontal)
			{
				if(topArea.endX - topArea.startX < 4)
				{
					m_splitPossible = false;
					return;
				}
				
				
				
				RoomArea area1 = new RoomArea();
				RoomArea area2 = new RoomArea();
				
				int range = topArea.endX - topArea.startX;
				
				range = range/ 2;
				
				if(range < minX)
				{
					m_sliceHorizontal = !m_sliceHorizontal;
					m_corridorsGenerated++;
					return;
				}
				
				int splitX = Random.Range(topArea.startX + range  /2, topArea.endX - range / 2);
				
				area1.startX = topArea.startX;
				area1.endX = splitX - 1;
				area1.startY = topArea.startY;
				area1.endY = topArea.endY;
				area1.m_type = RoomArea.SliceType.Vertical;
				
				area2.startX = splitX + 1;
				area2.endX = topArea.endX;
				area2.startY = topArea.startY;
				area2.endY = topArea.endY;
				area2.m_type = RoomArea.SliceType.Vertical;
				
				m_activeAreas.Add(area1);
				m_activeAreas.Add(area2);
				
				for(int x = splitX - 1; x < splitX + 1; x++)
				{
					for(int y = topArea.startY; y < topArea.endY; y++)
					{
						m_level.SetTileID(x, y, 1, false);
					}
				}
				m_corridorsGenerated++;
			}
			else
			{
				if(topArea.endY- topArea.startY < 4)
				{
					m_splitPossible = false;
					return;
				}
				
				RoomArea area1 = new RoomArea();
				RoomArea area2 = new RoomArea();
				
				int range = topArea.endY - topArea.startY;
				range = range/ 2;
				
				if(range < minY)
				{
					m_sliceHorizontal = !m_sliceHorizontal;
					m_corridorsGenerated++;
					return;
				}
				
				int splitY = Random.Range(topArea.startY + range  /2, topArea.endY - range / 2);
				
				
				area1.startX = topArea.startX;
				area1.endX = topArea.endX;
				area1.startY = topArea.startY;
				area1.endY = splitY - 1;
				area1.m_type = RoomArea.SliceType.Horizontal;
				
				area2.startX = topArea.startX;
				area2.endX = topArea.endX;
				area2.startY = splitY + 1;
				area2.endY = topArea.endY;
				area2.m_type = RoomArea.SliceType.Horizontal;
				
				m_activeAreas.Add(area1);
				m_activeAreas.Add(area2);
				
				for(int x = topArea.startX; x < topArea.endX; x++)
				{
					for(int y = splitY - 1; y < splitY + 1; y++)
					{
						m_level.SetTileID(x, y, 1, false);
					}
				}
				m_corridorsGenerated++;
			}
			
			m_sliceHorizontal = !m_sliceHorizontal;
				
		}
	}
	
	private Room GenerateRoomCandidate()
	{
		const int maxWidth = 20;
		const int maxHeight = 20;
		
		int width = Random.Range(6, maxWidth);
		int height = Random.Range((int)(width * 0.9f), (int)(width * 1.1f));
		
		int startX 	= (int)(Random.value * (m_levelWidth - width));
		int startY 	= (int)(Random.value * (m_levelHeight - height));
			
		Room newRoom = new Room();
		
	
		
		return newRoom;
	}
	
	private bool RoomOverlaps(Room room)
	{
		
		return false;
	}
	
	private RoomArea GetLargestArea(RoomArea.SliceType type)
	{
		RoomArea currentLargest = null;
		int currentMetric = int.MinValue;
		
		foreach(var area in m_activeAreas)
		{
			if(area.m_type != type)
			{
				continue;	
			}
			int metric = area.GetMetric();
			if(metric > currentMetric)
			{
				currentLargest = area;
				currentMetric = metric;
			}
		}
		return currentLargest;
	}
	
	private int m_targetRoomCount = 0;
	private int m_currentRoomCount = 0;
	private int m_levelWidth = 0;
	private int m_levelHeight = 0;
	private int m_roomCandidateIterationCount = 0;
	private bool m_roomFailed = false;
	private bool m_splitPossible = true;
	
	private int m_corridorsGenerated = 0;
	private int m_targetCorridorCount = 0;
	private bool m_sliceHorizontal = true;
	
	private List<RoomArea> m_activeAreas = new List<RoomArea>();
		
	private class RoomArea
	{
		public enum SliceType
		{
			Horizontal,
			Vertical,
			None
		}
		
		public SliceType m_type = SliceType.None;
		public int startX 	= 0;
		public int startY 	= 0;
		public int endX 	= 0;
		public int endY		= 0;
			
		public int GetMetric()
		{
			return (endX - startX) * (endY - startY);	
		}
	}
}

