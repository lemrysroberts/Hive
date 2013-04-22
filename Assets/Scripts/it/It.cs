using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class It : MonoBehaviour 
{
	public Level m_level = null;
	
	Mesh m_mesh = null;
	MeshFilter m_filter = null;
	
	// Use this for initialization
	void Start () 
	{
		m_filter = GetComponent<MeshFilter>();
		m_mesh = new Mesh();
		
		ThingTile newTile = new ThingTile();
		newTile.x = 10;
		newTile.y = 10;
		
		AddTile(10, 10);
		m_filter.mesh = m_mesh;
		
		m_openTiles.Add(newTile);
	}
	
	// Update is called once per frame
	float m_timer = 0.0f;
	void Update () 
	{
		m_timer += Time.deltaTime;
		if(m_timer > 1.0f)
		{
			m_timer = 0.0f;
			
			if(m_openTiles.Count > 0)
			{
//				ThingTile growTile = m_openTiles[(int)(Random.value * m_openTiles.Count)];
				
				// Look around it
				//if(
				
				
			}
			AddTile(10 + m_tileCount, 10);
				
		}
	}
	
	private void AddTile(int x, int y)
	{
		Vector3[] vertices 	= new Vector3[(m_tileCount + 1) * 4];
		Vector2[] uvs 		= new Vector2[(m_tileCount + 1) * 4];
		int[] triangles		= new int[(m_tileCount + 1) * 6];
		
		m_currentVertices.CopyTo(vertices, 0);
		m_currentUVs.CopyTo(uvs, 0);
		m_currentTriangles.CopyTo(triangles, 0);
		
		int index = m_tileCount * 4;	
		
		vertices[index] = new Vector3(x, y, 0.0f);
		vertices[index + 1] = new Vector3(x, y + 1.0f, 0.0f);
		vertices[index + 2] = new Vector3(x + 1.0f, y, 0.0f);
		vertices[index + 3] = new Vector3(x + 1.0f, y + 1.0f, 0.0f);
		
		index = m_tileCount * 6;
		triangles[index] = m_tileCount * 4;
		triangles[index + 1] = m_tileCount * 4 + 1;
		triangles[index + 2] = m_tileCount * 4 + 2;
		triangles[index + 3] = m_tileCount * 4 + 2;
		triangles[index + 4] = m_tileCount * 4 + 1;
		triangles[index + 5] = m_tileCount * 4 + 3;
		
		m_mesh.Clear();
		m_mesh.vertices = vertices;
		m_mesh.triangles = triangles;
		
		m_currentVertices = vertices;
		m_currentTriangles = triangles;
		
		m_tileCount++;
	}
	
	private int m_tileCount = 0;
	
	private Vector3[] m_currentVertices = new Vector3[0];
	private Vector2[] m_currentUVs = new Vector2[0];
	private int[] m_currentTriangles = new int[0];
	
	private List<ThingTile> m_openTiles = new List<ThingTile>();
}

public class ThingTile
{
	public int x;
	public int y;
}
