using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelSection : MonoBehaviour, IVisibilityReceiver
{
	public void RebuildData()
	{
		BuildMeshes();
	}
	
	public void RebuildColliders()
	{
		BuildColliders();
	}
	
	private void BuildMeshes()
	{
		// Initialise the tiles vectors if they're null
		if(m_tileIDs == null)	
		{
			m_tileIDs = new List<int>(m_sectionSize * m_sectionSize);
			
			for(int i = m_tileIDs.Count; i < m_sectionSize * m_sectionSize; i++)
			{
				m_tileIDs.Add(0);
			}
		}
		
		// Look for the "meshes" and flush it if found. Otherwise, create it.
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
		
		// Build a list of (point, tile-ID) pairs for tile batching.
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
		
		// Build the tile meshes
		
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
			newObject.name					= targetTile.TextureID;
			
			AnimatedTileMesh tileMesh 	= newObject.AddComponent<AnimatedTileMesh>();
			MeshRenderer renderer 		= newObject.AddComponent<MeshRenderer>();
			MeshFilter filter 			= newObject.AddComponent<MeshFilter>();
			
			newMesh.Optimize();						
			filter.mesh 			= newMesh;
			renderer.material 		= wallMaterial;
			tileMesh.SpriteDataPath = targetTile.SpriteDataPath;
		}
	}
	
	public void BuildColliders()
	{
		m_edges = GetEdges();
		
		Transform colliderObject = transform.FindChild("colliders");
		if(colliderObject != null)
		{
			while(colliderObject.GetChildCount() > 0)
			{
				DestroyImmediate(colliderObject.GetChild(0).gameObject);	
			}	
		}
		else
		{
			colliderObject = new GameObject("colliders").transform;
			colliderObject.transform.parent = transform;
			colliderObject.transform.position = m_origin;	
		}
		
		foreach(var edge in m_edges)
		{
			GameObject newObject 			= new GameObject();
			newObject.transform.parent 		= colliderObject.transform;
			newObject.transform.position 	= Origin + edge.Start + (edge.End - edge.Start) / 2.0f;
			newObject.name					= "Collider " + edge.Start.x + ", " + edge.Start.y;
				
			BoxCollider collider = newObject.AddComponent<BoxCollider>();
			Rigidbody rigidBody = newObject.AddComponent<Rigidbody>();
			
			rigidBody.isKinematic = true;
			rigidBody.constraints = RigidbodyConstraints.FreezeAll;
			
			Vector3 colliderSize = collider.size;
			colliderSize.x = edge.type == Edge.EdgeType.Horizontal ? (edge.End.x - edge.Start.x)  : 0.1f;
			colliderSize.y = edge.type == Edge.EdgeType.Vertical ? (edge.End.y - edge.Start.y) : 0.1f;
			
			newObject.transform.localScale = colliderSize;
		}
	}
	
	public List<Edge> GetEdges()
	{
		List<Edge> edges = new List<Edge>();
		List<bool> transition = new List<bool>();
		
		int originX = (int)m_origin.x;
		int originY = (int)m_origin.y;
		
		TileManager tileManager = TileManager.Instance;
		
		for(int x = 0; x < m_sectionSize ; x++)
		{
			for(int y = 0; y < m_sectionSize; y++)
			{
				int currentID 	= m_level.GetTileID(originX + x, 		originY + y);
				int nextXID 	= m_level.GetTileID(originX + x + 1, 	originY + y);
				int nextYID 	= m_level.GetTileID(originX + x, 		originY + y + 1);
				
				Tile currentTile = tileManager.GetTile(currentID);
				Tile nextXTile	 = nextXID != -1 ? tileManager.GetTile(nextXID) : null;
				Tile nextYTile	 = nextYID != -1 ? tileManager.GetTile(nextYID) : null;
				
				if(nextXTile != null && currentTile.NavBlock != nextXTile.NavBlock)
				{
					Edge newEdge = new Edge();
					newEdge.Start = new Vector2(x + 1.0f, y);
					newEdge.End = new Vector2(x + 1.0f, y + 1.0f);
					newEdge.type = Edge.EdgeType.Vertical;
					
					bool merged = false;
					foreach(Edge other in edges)
					{
						if(other.type == Edge.EdgeType.Vertical && (other.End.y == newEdge.Start.y || other.Start.y == newEdge.End.y) && other.Start.x == newEdge.Start.x)
						{
							other.Start.y 	= Mathf.Min(newEdge.Start.y, other.Start.y);
							other.End.y 	= Mathf.Max(newEdge.End.y, other.End.y);
							merged = true;
						}
					}
					
					if(!merged)
					{
						edges.Add(newEdge);
					}
				}
				
				if(nextYTile != null && currentTile.NavBlock != nextYTile.NavBlock)
				{
					Edge newEdge = new Edge();
					newEdge.Start = new Vector2(x, y + 1.0f);
					newEdge.End = new Vector2(x + 1.0f, y + 1.0f);
					newEdge.type = Edge.EdgeType.Horizontal;
					
					//Debug.Log("Collider found: " + (originX + x) + ", " + (originY + y) + " | " + (originX + x) + ", " + (originY + y + 1) + "  :  " + tileVal + ", " + nextYIndex);
					
					bool merged = false;
					foreach(Edge other in edges)
					{
						if(other.type == Edge.EdgeType.Horizontal && (other.End.x == newEdge.Start.x || other.Start.x == newEdge.End.x) && other.Start.y == newEdge.Start.y)
						{
							
							other.Start.x 	= Mathf.Min(newEdge.Start.x, other.Start.x);
							other.End.x 	= Mathf.Max(newEdge.End.x, other.End.x);
							merged = true;
						}
					}
					
					if(!merged)
					{
						edges.Add(newEdge);
					}
				}
			}
		}
		return edges;
	}
}