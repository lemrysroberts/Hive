#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class BinaryCorridors : IGeneratorStage
{
	public BinaryCorridors(Level level)
	{
		m_level = level;	
	}
	
	public void Start() 
	{
		m_levelWidth 	= m_level.SectionCountX * Level.m_sectionSize;
		m_levelHeight 	= m_level.SectionCountY * Level.m_sectionSize;
		m_splitPossible = true;
		
		// Create the initial division area from the entire level
		AreaPartition startArea = new AreaPartition();
		startArea.startX 		= 1;
		startArea.startY 		= 1;
		startArea.endX			= m_levelWidth - 2;
		startArea.endY			= m_levelHeight - 2;
		startArea.sliceType 	= AreaPartition.SliceType.Vertical;
		
		m_activeAreas.Add(startArea);
	}
	
	public void End()
	{
		m_level.RoomAreas = m_activeAreas;
		m_level.Corridors = m_corridors;
	}
	
	public void UpdateStep() 
	{
		GenerateCorridor();
	}
	
	public void UpdateAll() 
	{
		while(m_corridorsGenerated < m_maxCorridors && m_splitPossible)
		{
			GenerateCorridor();
		}
	}
	
	public bool StageComplete() 
	{ 
		return !m_splitPossible || m_corridorsGenerated == m_maxCorridors;	
	}
	
	public void SetupGUI()
	{
#if UNITY_EDITOR
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, GetStageName());
		if(m_showFoldout)
		{
			m_minX 			= EditorGUILayout.IntField("Area Minimum X", m_minX);	
			m_minY 			= EditorGUILayout.IntField("Area Minimum Y", m_minY);	
			m_maxCorridors 	= EditorGUILayout.IntField("Maximum Corridor Count", m_maxCorridors);	
			m_corridorWidth = EditorGUILayout.IntField("Corridor Width", m_corridorWidth);	
			
			m_minX 			= Mathf.Max(m_minX, 2);
			m_minY 			= Mathf.Max(m_minY, 2);
			m_maxCorridors 	= Mathf.Max(m_maxCorridors, 2);
			m_corridorWidth	= Mathf.Max(m_corridorWidth, 2);
			m_corridorWidth = (m_corridorWidth % 1 != 0) ? m_corridorWidth + 1 : m_corridorWidth;
		}
#endif
	}
	
	public void UpdateGUI() 
	{
		GUILayout.Label("Last Error: " + m_lastError);
	}
	
	public void UpdateSceneGUI() 
	{
#if UNITY_EDITOR
		AreaPartition topArea = GetLargestArea(m_sliceHorizontal ? AreaPartition.SliceType.Vertical : AreaPartition.SliceType.Horizontal);
		if(topArea != null)
		{
			Handles.color = Color.green;
			Handles.DrawLine(new Vector2(topArea.startX, topArea.startY), new Vector2(topArea.endX, topArea.startY));
			Handles.DrawLine(new Vector2(topArea.startX, topArea.startY), new Vector2(topArea.startX, topArea.endY));
			Handles.DrawLine(new Vector2(topArea.startX, topArea.endY), new Vector2(topArea.endX, topArea.endY));
			Handles.DrawLine(new Vector2(topArea.endX, topArea.startY), new Vector2(topArea.endX, topArea.endY));	
		}
#endif
	}
	
	public string GetStageName() { return "BSP Corridors"; }
	
	private void GenerateCorridor()
	{
		if(m_corridorsGenerated < m_maxCorridors && m_splitPossible)
		{
			AreaPartition topArea = GetLargestArea(m_sliceHorizontal ? AreaPartition.SliceType.Vertical : AreaPartition.SliceType.Horizontal);
			
			if(topArea == null)
			{
				m_splitPossible = false;
				return;
			}
			
			int startValue 	= m_sliceHorizontal ? topArea.startY : topArea.startX;
			int endValue	= m_sliceHorizontal ? topArea.endY : topArea.endX;
			
			int extents = endValue - startValue;
			
			// Determine the amount of space used by the walls and corridor, as these must be accounted for when checking for enough space.
			int usedSpace = m_corridorWidth;
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
				m_corridorsGenerated++;
				return;
			}
			
			// Determine a random split-location.
			int split = Random.Range(startValue + usedSpace / 2, endValue - usedSpace / 2);
			
			// Corridor			
			Corridor newCorridor = new Corridor();
			newCorridor.startX 	= m_sliceHorizontal ? topArea.startX : split - m_corridorWidth / 2;
			newCorridor.endX	= m_sliceHorizontal ? topArea.endX : split + m_corridorWidth / 2;
			newCorridor.startY 	= m_sliceHorizontal ? split - m_corridorWidth / 2 : topArea.startY;
			newCorridor.endY 	= m_sliceHorizontal ? split + m_corridorWidth / 2 : topArea.endY;
			
			m_corridors.Add(newCorridor);
			
			// New areas
			AreaPartition area1 = new AreaPartition();
			AreaPartition area2 = new AreaPartition();
			
			area1.startX 	= topArea.startX;
			area1.endX 		= m_sliceHorizontal ? topArea.endX : split - m_corridorWidth / 2;
			area1.startY 	= topArea.startY;
			area1.endY 		= m_sliceHorizontal ? split - m_corridorWidth / 2 : topArea.endY;
			area1.sliceType = m_sliceHorizontal? AreaPartition.SliceType.Horizontal : AreaPartition.SliceType.Vertical;
			
			area2.startX 	= m_sliceHorizontal ? topArea.startX : split + m_corridorWidth / 2;
			area2.endX 		= topArea.endX;
			area2.startY 	= m_sliceHorizontal ? split + m_corridorWidth / 2 : topArea.startY;
			area2.endY 		= topArea.endY;
			area2.sliceType = m_sliceHorizontal? AreaPartition.SliceType.Horizontal : AreaPartition.SliceType.Vertical;
			
			m_activeAreas.Add(area1);
			m_activeAreas.Add(area2);
			
			m_activeAreas.Remove(topArea);
			
			// Draw the corridor
			for(int x = newCorridor.startX; x < newCorridor.endX; x++)
			{
				for(int y = newCorridor.startY; y < newCorridor.endY; y++)
				{
					m_level.SetTileID(x, y, 1, false);
				}
			}
			
			m_corridorsGenerated++;
			m_sliceHorizontal = !m_sliceHorizontal;
				
		}
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
	
	private Level m_level 			= null;
	private int m_levelWidth 		= 0;
	private int m_levelHeight 		= 0;
	private bool m_splitPossible 	= true;
	private string m_lastError		= string.Empty;
	
	private int m_corridorsGenerated 	= 0;
	private bool m_sliceHorizontal 		= true;
	
	private static bool m_showFoldout = false;
	private static int m_minX 			= 8;
	private static int m_minY 			= 8;
	private static int m_maxCorridors	= 60;
	private static int m_corridorWidth 	= 2;
	
	private List<AreaPartition> m_activeAreas = new List<AreaPartition>();
	private List<Corridor> m_corridors = new List<Corridor>();
}

public class AreaPartition
{
	public enum SliceType
	{
		Horizontal,
		Vertical,
		None
	}
	
	public int failCount 		= 0;
	public SliceType sliceType 	= SliceType.None;
	public int startX 			= 0;
	public int startY 			= 0;
	public int endX 			= 0;
	public int endY				= 0;
		
	public int GetMetric()
	{
		return (endX - startX) * (endY - startY);	
	}
}

public class Corridor
{
	public int startX 			= 0;
	public int startY 			= 0;
	public int endX 			= 0;
	public int endY				= 0;
}
