using System;
using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public partial class LevelSection : MonoBehaviour, IVisibilityReceiver
{
	public GameObject Tile;
	
	public Level m_level;
	
	public List<Edge> m_edges = new List<Edge>();
	
	public void DrawDefaultGizmos()
	{
		Vector3 sectionCenter = m_origin;
		sectionCenter += new Vector3(m_sectionSize / 2, m_sectionSize / 2, 0.0f);
		
		Gizmos.color = new Color(0.9f, 0.9f, 0.9f, 0.3f);
		Gizmos.DrawWireCube(sectionCenter, new Vector3(m_sectionSize, m_sectionSize, m_sectionSize));
		
		for(int size = 0; size < m_sectionSize; size++)
		{
			Gizmos.color = new Color(0.7f, 0.7f, 0.7f, 0.1f);
			Gizmos.DrawLine(new Vector3(m_origin.x + size, m_origin.y, 0.0f), new Vector3(m_origin.x + size, m_origin.y + m_sectionSize, 0.0f));
			Gizmos.DrawLine(new Vector3(m_origin.x, m_origin.y + size, 0.0f), new Vector3(m_origin.x + m_sectionSize, m_origin.y + size, 0.0f));
		}
	}
	
	public void DrawColliderGizmos()
	{
		foreach(var edge in m_edges)
		{
			Gizmos.color = edge.type == Edge.EdgeType.Horizontal ? new Color(1.0f, 0.0f, 0.0f, 0.5f) : new Color(0.0f, 1.0f, 0.0f, 0.5f) ;
			Gizmos.DrawLine(new Vector3(m_origin.x + edge.Start.x, m_origin.y + edge.Start.y, 0.0f), new Vector3(m_origin.x + edge.End.x, m_origin.y + edge.End.y, 0.0f));
		}
	}
	
	public void AddObject(LevelObject newObject)
	{
		m_levelObjects.Add(newObject);
		/*
		GameObject subObjects = null;
		
		for(int childID = 0; childID < transform.GetChildCount() && subObjects == null; childID++)
		{
			Transform child = transform.GetChild(childID);
			if(child.name == m_objectsChildID)
			{
				subObjects = child.gameObject;
			}
		}
		
		if(subObjects == null)
		{
			subObjects = new GameObject(m_objectsChildID);
			subObjects.transform.parent = transform;
		}
		
		newObject.transform.parent = subObjects.transform;
		*/
	}
	
	public void CreateLevelObjects()
	{
		GameObject subObjects = null;
		
		for(int childID = 0; childID < transform.GetChildCount() && subObjects == null; childID++)
		{
			Transform child = transform.GetChild(childID);
			if(child.name == m_objectsChildID)
			{
				subObjects = child.gameObject;
			}
		}
		
		if(subObjects == null)
		{
			subObjects = new GameObject(m_objectsChildID);
			subObjects.transform.parent = transform;
		}
		
		WorldView view =  GameFlow.Instance.View;
		foreach(var levelObject in m_levelObjects)
		{
			GameObject newObject = null;
			
			switch(view)
			{
				case WorldView.Admin: newObject = levelObject.InstantiateAdmin(); break;
				case WorldView.Agent: newObject = levelObject.InstantiateAgent(); break;
			}
			
			if(newObject != null)
			{
				newObject.transform.parent = subObjects.transform;
			}
		}
	}
	
	public void ClearObjects()
	{
		m_levelObjects.Clear();
		
		GameObject subObjects = null;
		
		for(int childID = 0; childID < transform.GetChildCount() && subObjects == null; childID++)
		{
			Transform child = transform.GetChild(childID);
			if(child.name == m_objectsChildID)
			{
				subObjects = child.gameObject;
			}
		}
		
		if(subObjects != null)
		{
			for(int childID = 0; childID < subObjects.transform.GetChildCount(); childID++)
			{
				DestroyImmediate(subObjects.transform.GetChild(childID));
			}
		}
	}
	
	public void ElementVisible()
	{
		m_visibleSections++;
	}
		
	public void ElementInvisible()
	{
		m_visibleSections--;
		m_visibleSections = Math.Max(m_visibleSections, 0);
	}
	
	public Vector2 Origin
	{
		get { return m_origin; }
		set { m_origin = value; }
	}
	
	public int SectionSize
	{
		get { return m_sectionSize; }
		set { m_sectionSize = value; }
	}
	
	public List<int> TileIDs
	{
		get { return m_tileIDs; }
		set { m_tileIDs = value; }
	}
	
	[SerializeField]
	private Vector2 m_origin = new Vector2();
	
	[SerializeField]
	private int m_sectionSize = 0;
	
	public int m_visibleSections = 0;
	
	[SerializeField]
	private List<int> m_tileIDs;
			
	private const string m_objectsChildID = "objects";
	
	private class TilePointPair
	{
		public List<Vector2> m_points = new List<Vector2>();
		public int m_tileID = -1;
	}
	
	private List<LevelObject> m_levelObjects = new List<LevelObject>();
} 

[Serializable]
public class Edge
{
	public enum EdgeType
	{
		Vertical,
		Horizontal
	}
	
	[SerializeField]
	public Vector2 Start 	= new Vector2();
	
	[SerializeField]
	public Vector2 End 		= new Vector2();
	
	[SerializeField]
	public EdgeType type;
}
