using System;
using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public partial class LevelSection : MonoBehaviour, IVisibilityReceiver
{
	public GameObject Tile;
	
	public void Start()
	{
		
	}
	
	public void RebuildData()
	{
		if(m_tileIDs == null)	
		{
			m_tileIDs = new List<int>(m_sectionSize * m_sectionSize);
			m_tileNavStates = new List<NavState>(m_sectionSize * m_sectionSize);
			
			for(int i = m_tileIDs.Count; i < m_sectionSize * m_sectionSize; i++)
			{
				m_tileIDs.Add(0);
				m_tileNavStates.Add(NavState.LayoutBlocked);
			}
		}
		
		Transform meshObject = transform.FindChild("meshes");
		if(meshObject != null)
		{
			while(meshObject.GetChildCount() > 0)
			{
				DestroyImmediate(meshObject.GetChild(0).gameObject);	
			}	
		}
		else
		{
			meshObject = new GameObject("meshes").transform;
			meshObject.transform.parent = transform;
			meshObject.transform.position = m_origin;	
			
		}
		
		List<TilePointPair> pairs = new List<TilePointPair>();
		
		for(int x = 0; x < m_sectionSize; x++)
		{
			for(int y = 0; y < m_sectionSize; y++)
			{
				TilePointPair pair = null;
				foreach(var currentPair in pairs)
				{
					if(currentPair.m_tileID == m_tileIDs[x * m_sectionSize + y])
					{
						pair = currentPair;
						break;
					}
				}
				
				if(pair == null)
				{
					pair = new TilePointPair();
					pair.m_tileID = m_tileIDs[x * m_sectionSize + y];
					pairs.Add(pair);
				}
				
				pair.m_points.Add(new Vector2(x, y));	
			}
		}
		
		foreach(var pair in pairs)
		{
			Tile targetTile = TileManager.Instance.GetTile(pair.m_tileID);
			
			if(targetTile == null)
				continue;
			
			Material wallMaterial = targetTile.GetMaterial();
			
			Mesh newMesh = new Mesh();
			
			Vector3[] vertices = new Vector3[pair.m_points.Count * 4];
			Vector2[] uvs = new Vector2[pair.m_points.Count * 4];
			int[] triangles = new int[pair.m_points.Count * 6];
			
			int id = 0;
			foreach(var point in pair.m_points)
			{
				vertices[id * 4] = point;
				vertices[id * 4 + 1] = point + new Vector2(0.0f, 1.0f);
				vertices[id * 4 + 2] = point + new Vector2(1.0f, 0.0f);
				vertices[id * 4 + 3] = point + new Vector2(1.0f, 1.0f);
				
				uvs[id * 4] = new Vector2(0.0f, 0.0f);
				uvs[id * 4 + 1] = new Vector2(0.0f, 1.0f);
				uvs[id * 4 + 2] = new Vector2(1.0f, 0.0f);
				uvs[id * 4 + 3] = new Vector2(1.0f, 1.0f);
				
				triangles[id * 6] = id * 4;
				triangles[id * 6 + 1] = id * 4 + 1;
				triangles[id * 6 + 2] = id * 4 + 2;
				
				triangles[id * 6 + 3] = id * 4 + 2;
				triangles[id * 6 + 4] = id * 4 + 1;
				triangles[id * 6 + 5] = id * 4 + 3;
				id++;
			}
			
			newMesh.vertices = vertices;
			newMesh.triangles = triangles;
			newMesh.uv = uvs;
			
			GameObject newObject 			= new GameObject();
			newObject.transform.parent 		= meshObject.transform;
			newObject.transform.position 	= meshObject.transform.position;
			
			AnimatedTileMesh tileMesh 	= newObject.AddComponent<AnimatedTileMesh>();
			MeshRenderer renderer 		= newObject.AddComponent<MeshRenderer>();
			MeshFilter filter 			= newObject.AddComponent<MeshFilter>();
			
			newMesh.Optimize();						
			filter.mesh 			= newMesh;
			renderer.material 		= wallMaterial;
			tileMesh.SpriteDataPath = targetTile.SpriteDataPath;
		}
	}
	 
	public void OnDrawGizmos()
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
	
	public List<NavState> NavStates
	{
		get { return m_tileNavStates; }
		set { m_tileNavStates = value; }	
	}
	
	[SerializeField]
	private Vector2 m_origin = new Vector2();
	
	[SerializeField]
	private int m_sectionSize = 0;
	
	public int m_visibleSections = 0;
	
	[SerializeField]
	private List<int> m_tileIDs;
	
	[SerializeField]
	private List<NavState> m_tileNavStates;
	
	private class TilePointPair
	{
		public List<Vector2> m_points = new List<Vector2>();
		public int m_tileID = -1;
	}
}
