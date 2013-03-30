using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public partial class LevelSection : MonoBehaviour, IVisibilityReceiver
{
	public GameObject Tile;
	
	[SerializeField]
	private VisibilityReporter m_visReporter = null;
	
	public void Start()
	{
		m_visReporter.RegisterReceiver(this);
	}
	
	public void RebuildData()
	{
		if(m_tileIDs == null)	
			m_tileIDs = new List<int>(m_sectionSize * m_sectionSize);
		
		for(int i = m_tileIDs.Count; i < m_sectionSize * m_sectionSize; i++) m_tileIDs.Add(0);
		
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
		
		for(int i = 0; i < 2; i++)
		{
			List<Vector2> points = new List<Vector2>();
			
			for(int x = 0; x < m_sectionSize; x++)
			{
				for(int y = 0; y < m_sectionSize; y++)
				{
					if(m_tileIDs[x * m_sectionSize + y] == i)
					{
						points.Add(new Vector2(x, y));	
					}
				}
			}
			
			Material wallMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Tile_" + i + ".mat", typeof(Material));
			
			Mesh newMesh = new Mesh();
			
			Vector3[] vertices = new Vector3[points.Count * 4];
			Vector2[] uvs = new Vector2[points.Count * 4];
			int[] triangles = new int[points.Count * 6];
			
			for(int id = 0; id < points.Count; id++)
			{
				vertices[id * 4] = points[id];
				vertices[id * 4 + 1] = points[id] + new Vector2(0.0f, 1.0f);
				vertices[id * 4 + 2] = points[id] + new Vector2(1.0f, 0.0f);
				vertices[id * 4 + 3] = points[id] + new Vector2(1.0f, 1.0f);
				
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
			}
			
			newMesh.vertices = vertices;
			newMesh.triangles = triangles;
			newMesh.uv = uvs;
			
			GameObject newObject = new GameObject();
			newObject.transform.parent = meshObject.transform;
			newObject.transform.position = meshObject.transform.position;
			
			MeshRenderer renderer = newObject.AddComponent<MeshRenderer>();
			MeshFilter filter = newObject.AddComponent<MeshFilter>();
			m_visReporter = newObject.AddComponent<VisibilityReporter>();
						
			//newMesh.Optimize();
			filter.mesh = newMesh;
			renderer.material = wallMaterial;
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
	
	public void FixedUpdate()
	{
		
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
}
