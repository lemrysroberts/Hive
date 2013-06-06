using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelSection : MonoBehaviour, IVisibilityReceiver
{
	public void InitTileData()
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
	}
	
	public void RebuildData()
	{
		InitTileData();
		BuildMeshes();
	}
	
	// TODO: Why is this a separate function from BuildColliders :/
	public void RebuildColliders()
	{
		if(GameFlow.Instance.View == WorldView.Agent)
		{
			BuildColliders();
		}
		else
		{
			Debug.Log("Admin view selected. Skipping collider generation.");
		}
	}
	
	private void BuildMeshes()
	{
		
		
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
		
		if(GameFlow.Instance.View == WorldView.Admin)
		{
			List<Edge> edges = GetEdges();
			Mesh newMesh = new Mesh();
				
			Vector3[] vertices = new Vector3[edges.Count * 4];
			Vector2[] uvs = new Vector2[edges.Count * 4];
			int[] triangles = new int[edges.Count * 6];
			
			float wallWidth = 0.05f;
			
			int id = 0;
			foreach(var edge in edges)
			{
				vertices[id * 4] = new Vector3(edge.Start.x - wallWidth, edge.Start.y - wallWidth, 0.0f);
				vertices[id * 4 + 1] = new Vector3(edge.Start.x - wallWidth, edge.End.y + wallWidth, 0.0f);
				vertices[id * 4 + 2] = new Vector3(edge.End.x + wallWidth, edge.Start.y - wallWidth, 0.0f);
				vertices[id * 4 + 3] = new Vector3(edge.End.x + wallWidth, edge.End.y + wallWidth, 0.0f);
				
				uvs[id * 4] = (Vector2)(vertices[id * 4]);
				uvs[id * 4 + 1] = (Vector2)(vertices[id * 4 + 1]);
				uvs[id * 4 + 2] = (Vector2)(vertices[id * 4 + 2]);
				uvs[id * 4 + 3] = (Vector2)(vertices[id * 4 + 3]);
				
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
			
			MeshRenderer renderer 		= newObject.AddComponent<MeshRenderer>();
			MeshFilter filter 			= newObject.AddComponent<MeshFilter>();
			
			newMesh.Optimize();						
			filter.mesh 			= newMesh;
			renderer.sharedMaterial = AssetHelper.Instance.GetAsset<Material>("Materials/AdminWall") as Material;
		}
		else
		{
			
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
				newObject.transform.position 	= meshObject.transform.position + new Vector3(0.0f, 0.0f, -targetTile.Elevation);
				newObject.name					= targetTile.TextureID;
				
				AnimatedTileMesh tileMesh 	= newObject.AddComponent<AnimatedTileMesh>();
				MeshRenderer renderer 		= newObject.AddComponent<MeshRenderer>();
				MeshFilter filter 			= newObject.AddComponent<MeshFilter>();
				
				newMesh.Optimize();						
				filter.mesh 			= newMesh;
				renderer.material 		= wallMaterial;
				tileMesh.SpriteDataPath = targetTile.SpriteDataPath;
				tileMesh.AnimationSpeed = targetTile.AnimationSpeed;
			}
		}
	}
	
	public void BuildColliders()
	{
		List<TileAccessNode> walls = new List<TileAccessNode>();
		
		// Iterate horizontally, looking up each column of tiles and building walls along y.
		TileAccessNode currentNode = null;
		for(int x = 0; x < m_sectionSize; x++)
		{
			// At the start of each column, assume no active wall.
			currentNode = null;
			for(int y = 0; y < m_sectionSize; y++)
			{
				if(m_level.TileBlocked((int)Origin.x + x, (int)Origin.y + y))
				{
					// If no wall is being made, create one.
					if(currentNode == null)
					{
						currentNode = new TileAccessNode();
						currentNode.min = new Vector2(x, y);
						currentNode.max = currentNode.min + new Vector2(1.0f, 1.0f);
					}
					
					currentNode.max.y = y + 1.0f;
				}
				else
				{
					// If no wall should be made but one exists, add it to the wall list and clear the current wall.
					if(currentNode != null)
					{
						walls.Add(currentNode);
						currentNode = null;
					}
				}
			}
			if(currentNode != null)
			{
				// If the current vertical search has ended, add the active wall to the walls list
				walls.Add(currentNode);
				currentNode = null;
			}
		}
		
		// At this stage, the list of all vertical walls has been made.
		// The next step is to try and merge these walls horizontally.
		// This strategy is hardly optimal as it acts in separate x,y axis, meaning it is ignorant of rectangular optimisation.
		// The thing is though, right, that I don't care at all.
		
		// This is required to store dead walls, as C# hates me modifying enumerations as I iterate over them. Which is fair.
		List<TileAccessNode> toDelete = new List<TileAccessNode>();
		
		bool merged = true;
		while(merged)
		{
			merged = false;
			
			foreach(var wall in walls)
			{
				// Ignore isolated and dead walls.
				if(!wall.canMerge) continue; 
				
				bool mergeFound = false;
				foreach(var other in walls)
				{
					if(!wall.canMerge) 	continue;
					if(other == wall)	continue; 
						
					// If the walls share an x-value and exactly match in y, they can be merged.
					if( (other.min.x == wall.max.x || other.max.x == wall.min.x) &&
						(other.min.y == wall.min.y) && (other.max.y == wall.max.y))
					{
						// Retain one wall with merged dimensions
						wall.min.x = Math.Min(wall.min.x, other.min.x);
						wall.max.x = Math.Max(wall.max.x, other.max.x);
						
						mergeFound 		= true;
						merged 			= true;
						other.canMerge 	= false;
						
						// Set the partner of the merge to be dead.
						toDelete.Add(other);
						break;
					}
				}
				
				if(!mergeFound)
				{
					// If no merges were found, mark the wall to save checking it again.
					wall.canMerge = false;	
				}
			}
		}
		
		// Clear dead walls.
		foreach(var wall in toDelete)
		{
			walls.Remove(wall);	
		}
		
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
		
		foreach(var wall in walls)
		{
			GameObject newObject 			= new GameObject();
			newObject.transform.parent 		= colliderObject.transform;
			newObject.transform.position 	= Origin + wall.min + (wall.max - wall.min) / 2.0f;
			newObject.name					= "Collider " + wall.min.x + ", " + wall.min.y;
				
			BoxCollider collider = newObject.AddComponent<BoxCollider>();
			newObject.layer = LayerMask.NameToLayer("LevelGeo");
			
			//Rigidbody test = newObject.AddComponent<Rigidbody>();
			//test.isKinematic = true;
			
			Vector3 colliderSize = collider.size;
			colliderSize.x = (wall.max.x - wall.min.x);
			colliderSize.y = (wall.max.y - wall.min.y);
			colliderSize.z = 5;
			
			newObject.transform.localScale = colliderSize;
		}
	}
	
	public List<Edge> GetEdges()
	{
		List<Edge> edges = new List<Edge>();
		
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
	
	private class TileAccessNode
	{
		public bool canMerge = true;
		public Vector2 min;
		public Vector2 max;
	}
}
