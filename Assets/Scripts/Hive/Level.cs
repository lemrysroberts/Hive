/// <summary>
/// Level.cs
/// 
/// This contains all level-related data.
/// The level is built of LevelSection instances in order to allow simple LOD behaviour at some murky point in the future.
/// 
/// TODO: Despite being split into partial classes, this is getting pretty bloated.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NavState
{
	Open,
	LayoutBlocked,
	ThingBlocked
}

[ExecuteInEditMode]
public partial class Level : MonoBehaviour 
{
	void Start()
	{
		m_previousSectionCountX = SectionCountX;	
		m_previousSectionCountY = SectionCountY;
		
		if(m_graph == null)
		{
			RebuildAIGraph();	
		}
	}

	/// <summary>
	/// Recreates the level-sections.
	/// </summary>
	public void Reload(bool fullReload)
	{
		LevelNetwork.Instance.Reset();
		
		DeleteSections(fullReload);
		Transform sectionsTransform = CreateSectionsObject().transform;
		
		List<LevelSection> newSections = new List<LevelSection>();
		for(int i = 0; i < SectionCountX * SectionCountY; i++) newSections.Add(null);
		{
			for(int x = 0; x < SectionCountX; x++)
			{
				for(int y = 0; y < SectionCountY; y++)
				{
					if( x < m_previousSectionCountX && 
						y < m_previousSectionCountY && 
						x < SectionCountX &&
						y < SectionCountY &&
						m_sections.Count > 0)
					{
						newSections[x * SectionCountY + y] = m_sections[x * m_previousSectionCountY + y];	
					}
					
					bool sectionExists = false;
					if(!fullReload)
					{
						for(int childIndex = 0; childIndex < sectionsTransform.GetChildCount(); childIndex++)
						{
							LevelSection otherSection = sectionsTransform.GetChild(childIndex).gameObject.GetComponent<LevelSection>();
							if(otherSection.Origin.x == m_sectionSize * x && otherSection.Origin.y == m_sectionSize * y)
							{
								sectionExists = true;
							}
						}
					}
					
					if(sectionExists)
					{
						continue;	
					}
						
					GameObject newSection = new GameObject("Section " + x + ", " + y);
					newSection.transform.parent = sectionsTransform;
					
					LevelSection section = newSection.AddComponent<LevelSection>();
					
					section.m_level = this;
					newSection.tag = "level_section";
			
					BoxCollider collider 	= newSection.AddComponent<BoxCollider>();
					Rigidbody rigidBody 	= newSection.AddComponent<Rigidbody>(); // I hate this
					m_visReporter 			= newSection.AddComponent<VisibilityReporter>();
					m_visReporter.RegisterReceiver(section);
					
					rigidBody.isKinematic = true;
					
					collider.isTrigger = true;
					collider.center = new Vector3(m_sectionSize / 2, m_sectionSize / 2, 0.0f);
					collider.size 	= new Vector3(m_sectionSize, m_sectionSize, 10.0f); // The 10.0f is fairly arbitrary
					
					section.Origin = new Vector2(x * m_sectionSize, y * m_sectionSize);
					section.SectionSize = m_sectionSize;
					section.Tile = TileType;
					section.RebuildData();
					
					newSections[x * SectionCountY + y] = section;
				}
			}
			
			m_sections = newSections; 
		}
		
		m_previousSectionCountX = SectionCountX;
		m_previousSectionCountY = SectionCountY;
	}
	
	public void RebuildAllSections()
	{
		foreach(var section in m_sections)
		{
			section.RebuildData();
		}
		
		// TODO: Split off the child deletion gubbins in GetLevelChild, so these calls are so inscrutable.
		GetLevelChild("AdminRooms", true);
		GetLevelChild("AdminCorridors", true);
		
		if(GameFlow.Instance.View == WorldView.Admin)
		{
			RebuildAdminRooms();
			RebuildAdminCorridors();
		}
	}
	
	public void SetTileID(int x, int y, int tileID, bool rebuild)
	{
		int sectionIDX = x / m_sectionSize;
		int sectionIDY = y / m_sectionSize;
		
		int localIDX = x % m_sectionSize;
		int localIDY = y % m_sectionSize; 
		
		LevelSection section = m_sections[sectionIDX * SectionCountY + sectionIDY];
		
		section.TileIDs[localIDX * m_sectionSize + localIDY] = tileID; 
		
		if(rebuild)
			section.RebuildData();
	}
	
	public int GetTileID(int x, int y)
	{
		int sectionIDX = x / m_sectionSize;
		int sectionIDY = y / m_sectionSize;
		
		int localIDX = x % m_sectionSize;
		int localIDY = y % m_sectionSize; 
		
		if(sectionIDX >= SectionCountX || sectionIDY >= SectionCountY || localIDX >= m_sectionSize || localIDY >= m_sectionSize)
		{
			return -1;	
		}
		
		LevelSection section = m_sections[sectionIDX * SectionCountY + sectionIDY];
		
		return section.TileIDs[localIDX * m_sectionSize + localIDY]; 
	}
	
	public void RebuildAIGraph()
	{
		m_graph = BuildAIGraph();	
	}
	
	public void RebuildColliders()
	{
		foreach(var section in m_sections)
		{
			section.RebuildColliders();
		}
	}
	
	public void SetNetworkViewEnabled(bool enabled)
	{
		GameObject networkObject = GetNetworkParentObject(false);
		networkObject.SetActive(enabled);
	}
	
	public GameObject GetNetworkParentObject(bool deleteChildren)
	{
		const string networkStringID = "LevelNetwork";
		GameObject networkObject = null;
		
		for(int childIndex = 0; childIndex < transform.childCount && networkObject == null; childIndex++)
		{
			GameObject current = transform.GetChild(childIndex).gameObject;
			if(current.name == networkStringID)
			{
				networkObject = current;	
			}
		}
		
		if(networkObject == null)
		{
			networkObject = new GameObject(networkStringID);	
			networkObject.transform.parent = transform;
			networkObject.transform.position += new Vector3(0.0f, 0.0f, -2.0f);
		}
		
		if(deleteChildren)
		{
			while(networkObject.transform.GetChildCount() > 0)
			{
				DestroyImmediate(networkObject.transform.GetChild(0).gameObject);
			}
		}
		
		return networkObject;
	}
	
	public bool TileBlocked(int x, int y)
	{
		Tile tile = TileManager.Instance.GetTile(GetTileID(x, y));
		return tile.NavBlock;
	}
	
	public void CreateLevelObjects()
	{
		Dictionary<int, NetworkViewID> levelObjectIDs = new Dictionary<int, NetworkViewID>();
		foreach(var section in m_sections)
		{
			Dictionary<int, NetworkViewID> sectionIDs = section.CreateLevelObjects();
			foreach(var id in sectionIDs)
			{
				levelObjectIDs.Add(id.Key, id.Value);
			}
		}
		m_levelObjectIDs = levelObjectIDs;
	}
	
	private void DeleteSections(bool fullReload)
	{
		Transform sectionsTransform = transform.FindChild(s_sectionsID);
		
		if(sectionsTransform != null)
		{
			List<LevelSection> toDelete = new List<LevelSection>();
			
			for(int childIndex = 0; childIndex < sectionsTransform.GetChildCount(); childIndex++)
			{
				LevelSection section = sectionsTransform.GetChild(childIndex).gameObject.GetComponent<LevelSection>();
				if(section.Origin.x >= m_sectionSize * SectionCountX || section.Origin.y >= m_sectionSize * SectionCountY || fullReload)
				{
					toDelete.Add(section);	
				}
			}
			
			foreach(var section in toDelete) 
			{
				DestroyImmediate(section.gameObject);	
			}	
		}
	}
	
	private GameObject CreateSectionsObject()
	{
		Transform sectionsTransform = transform.FindChild(s_sectionsID);
		
		if(sectionsTransform == null)
		{
			GameObject sectionsObject = new GameObject(s_sectionsID);
			sectionsTransform = sectionsObject.transform;
			sectionsTransform.parent = transform;
		}
		
		return sectionsTransform.gameObject;
	}
	
	private GameObject GetLevelChild(string childName, bool clearChildren)
	{
		GameObject childObject = null;
		
		for(int childIndex = 0; childIndex < transform.childCount && childObject == null; childIndex++)
		{
			GameObject current = transform.GetChild(childIndex).gameObject;
			if(current.name == childName)
			{
				childObject = current;	
			}
		}
		
		if(childObject == null)
		{
			childObject = new GameObject(childName);	
			childObject.transform.parent = transform;
			childObject.transform.position += new Vector3(0.0f, 0.0f, 0.0f);
		}
		
		while(childObject.transform.GetChildCount() > 0)
		{
			DestroyImmediate(childObject.transform.GetChild(0).gameObject);
		}
		
		return childObject;
	}
	
	private AIGraph BuildAIGraph()
	{
		AIGraph newGraph = new AIGraph(SectionCountX * m_sectionSize, SectionCountY * m_sectionSize);
		
		TileManager tileManager = TileManager.Instance;
		
		for(int x = 0; x < SectionCountX * m_sectionSize; x++)
		{
			for(int y = 0; y < SectionCountY * m_sectionSize; y++)
			{
				Tile currentTile = tileManager.GetTile(GetTileID(x, y));
				
				if(currentTile != null && !currentTile.NavBlock)
				{
					AIGraphNode newNode = new AIGraphNode();
					newNode.NodePosition = new Vector2(x + 0.5f, y + 0.5f);
					
					newGraph.Add(newNode);
				}
			}
		}
		
		// Link the nodes. This should live in AIGraph really.
		foreach(var node in newGraph.Nodes)
		{
			if(node != null)
			{
				
				int currentIndex = newGraph.GetNodeIndex(node.NodePosition);
				
				int[] indices = new int[4];
				
				indices[0] = currentIndex + SectionCountX * m_sectionSize;
				indices[1] = currentIndex - SectionCountX * m_sectionSize;
				indices[2] = currentIndex + 1;
				indices[3] = currentIndex - 1;
				
				foreach(int otherIndex in indices)
				{
					if(otherIndex >= 0 && otherIndex < (SectionCountX * m_sectionSize) * (SectionCountY * m_sectionSize))
					{
						var other = newGraph.Nodes[otherIndex];
						if(other != null)
						{
							node.NodeLinks.Add(other);	
						}	
					}
				}
			}
		}
		
		return newGraph;
	}
	
	public void ClearObjects()
	{
		foreach(var section in m_sections)
		{
			section.ClearObjects();
		}
	}
	
	public void AddLevelObject(LevelObject newObject)
	{
		m_levelObjects.Add(newObject);
		
		LevelSection targetSection = GetLevelSection(newObject.Position);
		if(targetSection == null)
		{
			Debug.LogError("Failed to add GameObject: " + newObject.ToString() + ". Has its Position parameter been set? This is needed to determine its target section.");	
			return;
		}
		
		targetSection.AddObject(newObject);
	}
	
	public LevelSection GetLevelSection(Vector2 position)
	{
		int sectionIDX = (int)position.x / m_sectionSize;
		int sectionIDY = (int)position.y / m_sectionSize;
		
		if(sectionIDX >= SectionCountX || sectionIDY >= SectionCountY)
		{
			return null;	
		}
		
		return m_sections[sectionIDX * SectionCountY + sectionIDY];
	}
	
	public void TestRoutefinder()
	{
		/*
		RouteFinder routeFinder = new RouteFinder();
		
		int endPosID = (int)(Random.value * (float)(m_graph.Nodes.Count - 1));
		if(m_graph != null && m_graph.Nodes.Count > endPosID)
		{
			m_lastRouteStart = m_graph.Nodes[0].NodePosition;
			m_lastRouteEnd = m_graph.Nodes[endPosID].NodePosition;
			m_lastRoute = routeFinder.FindRoute(m_graph, m_graph.Nodes[0], m_graph.Nodes[endPosID]);
		}
		else
		{
			Debug.Log("Invalid graph state");	
		}
		*/
	}
	
	private void RebuildAdminRooms()
	{
		if(m_rooms == null)
		{
			return;	
		}
		
		GameObject roomsObject = GetLevelChild("AdminRooms", true);
		
		
		Mesh newMesh = new Mesh();
		
		const int triangleVertexCount = 9;
		
		Vector3[] vertices = new Vector3[m_rooms.Count * 4];
		Vector2[] uvs = new Vector2[m_rooms.Count * 4];
		int[] triangles = new int[m_rooms.Count * triangleVertexCount];
		
		int id = 0;
		foreach(var room in m_rooms)
		{
			vertices[id * 4] = new Vector3(room.startX, room.startY, 0.0f);
			vertices[id * 4 + 1] = new Vector3(room.startX, room.endY, 0.0f);
			vertices[id * 4 + 2] = new Vector3(room.endX, room.startY, 0.0f);
			vertices[id * 4 + 3] = new Vector3(room.endX, room.endY, 0.0f);
			
			uvs[id * 4] 	= (Vector2)(vertices[id * 4]);
			uvs[id * 4 + 1] = (Vector2)(vertices[id * 4 + 1]);
			uvs[id * 4 + 2] = (Vector2)(vertices[id * 4 + 2]);
			uvs[id * 4 + 3] = (Vector2)(vertices[id * 4 + 3]);
			
			triangles[id * triangleVertexCount] = id * 4;
			triangles[id * triangleVertexCount + 1] = id * 4 + 1;
			triangles[id * triangleVertexCount + 2] = id * 4 + 2;
			triangles[id * triangleVertexCount + 3] = id * 4 + 2;
			triangles[id * triangleVertexCount + 4] = id * 4 + 1;
			triangles[id * triangleVertexCount + 5] = id * 4 + 3;
			triangles[id * triangleVertexCount + 6] = id * 4 + 3;
			triangles[id * triangleVertexCount + 7] = id * 4 + 3;
			id++;
		}
		
		newMesh.vertices = vertices;
		newMesh.triangles = triangles;
		newMesh.uv = uvs;
		
		GameObject newObject 			= new GameObject();
		newObject.transform.parent = roomsObject.transform;
		
		MeshRenderer renderer 		= newObject.AddComponent<MeshRenderer>();
		MeshFilter filter 			= newObject.AddComponent<MeshFilter>();
		
		newMesh.Optimize();						
		filter.mesh 			= newMesh;
		renderer.sharedMaterial = AssetHelper.Instance.GetAsset<Material>("Materials/AdminRoom") as Material;
	}
	
	private void RebuildAdminCorridors()
	{
		if(Corridors == null)
		{
			return;	
		}
		
		GameObject corridorsObject = GetLevelChild("AdminCorridors", true);
		
		Mesh newMesh = new Mesh();
		
		const int triangleVertexCount = 9;
		
		Vector3[] vertices = new Vector3[Corridors.Count * 4];
		Vector2[] uvs = new Vector2[Corridors.Count * 4];
		int[] triangles = new int[Corridors.Count * triangleVertexCount];
		
		int id = 0;
		foreach(var corridor in Corridors)
		{
			vertices[id * 4] = new Vector3(corridor.startX, corridor.startY, 0.0f);
			vertices[id * 4 + 1] = new Vector3(corridor.startX, corridor.endY, 0.0f);
			vertices[id * 4 + 2] = new Vector3(corridor.endX, corridor.startY, 0.0f);
			vertices[id * 4 + 3] = new Vector3(corridor.endX, corridor.endY, 0.0f);
			
			uvs[id * 4] 	= (Vector2)(vertices[id * 4]);
			uvs[id * 4 + 1] = (Vector2)(vertices[id * 4 + 1]);
			uvs[id * 4 + 2] = (Vector2)(vertices[id * 4 + 2]);
			uvs[id * 4 + 3] = (Vector2)(vertices[id * 4 + 3]);
			
			triangles[id * triangleVertexCount] = id * 4;
			triangles[id * triangleVertexCount + 1] = id * 4 + 1;
			triangles[id * triangleVertexCount + 2] = id * 4 + 2;
			triangles[id * triangleVertexCount + 3] = id * 4 + 2;
			triangles[id * triangleVertexCount + 4] = id * 4 + 1;
			triangles[id * triangleVertexCount + 5] = id * 4 + 3;
			triangles[id * triangleVertexCount + 6] = id * 4 + 3;
			triangles[id * triangleVertexCount + 7] = id * 4 + 3;
			id++;
		}
		
		newMesh.vertices = vertices;
		newMesh.triangles = triangles;
		newMesh.uv = uvs;
		
		GameObject newObject 		= new GameObject();
		newObject.transform.parent 	= corridorsObject.transform;
		
		MeshRenderer renderer 		= newObject.AddComponent<MeshRenderer>();
		MeshFilter filter 			= newObject.AddComponent<MeshFilter>();
		
		newMesh.Optimize();						
		filter.mesh 			= newMesh;
		renderer.sharedMaterial = AssetHelper.Instance.GetAsset<Material>("Materials/AdminCorridor") as Material;
	}
	
	// TODO: These don't need to be properties. 
	public AIGraph AIGraph
	{
		get { return m_graph; }
	}
	
	public List<Room> Rooms
	{
		get { return m_rooms; }
		set { m_rooms = value; }
	}
	
	
	public Vector2 PlayerSpawnPoint
	{
		get { return m_playerSpawnPoint; }
		set { m_playerSpawnPoint = value; }
	}
	
	public Vector2 GoalItemSpawnPoint
	{
		get { return m_goalSpawnPoint; }
		set { m_goalSpawnPoint = value; }
	}
	
	public int NPCCount
	{
		get { return m_npcCount; }
		set { m_npcCount = value; }
	}		
	
	public int Width { get { return m_sectionSize * SectionCountX; } }
	public int Height { get { return m_sectionSize * SectionCountY; } }
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if(m_sections != null)
		{
			foreach(var section in m_sections)
			{
				if(section != null)
				{
					section.DrawDefaultGizmos();	 
					
					if(m_renderColliders)
					{
						section.DrawColliderGizmos();	
					}
				}
			}
		}
		
		if(m_graph != null && m_renderNodeGraph)
		{
			Gizmos.color = Color.cyan;
			
			Vector3 boxSize = new Vector3(0.2f, 0.2f, 0.2f);
			foreach(var node in m_graph.Nodes)
			{
				if(node != null)
				{
					Gizmos.DrawCube(node.NodePosition, boxSize);
					
					foreach(var other in node.NodeLinks)
					{
						Gizmos.DrawLine(node.NodePosition, other.NodePosition);	
					}
				}
			}
			
			Gizmos.color = Color.magenta;
			boxSize = new Vector3(0.3f, 0.3f, 0.3f);
			if(m_lastRoute != null)
			{
				for(int i = 0; i < m_lastRoute.m_routePoints.Count; i++)
				{
					Vector2 point = m_lastRoute.m_routePoints[i].NodePosition;
					Vector2 altPoint = i > 0 ? m_lastRoute.m_routePoints[i - 1].NodePosition : m_lastRoute.m_routePoints[i].NodePosition;
					
					Gizmos.DrawCube(point, boxSize);
					Gizmos.DrawLine(point, altPoint);
				}
				
				Gizmos.color = Color.green;
				boxSize = new Vector3(0.4f, 0.4f, 0.3f);
			}
		}
		
		if(m_renderRooms)
		{
			Gizmos.color = Color.magenta;
			foreach(var room in m_rooms)
			{
				Vector3 pos = new Vector3(room.startX + (room.endX - room.startX) / 2.0f, room.startY + (room.endY - room.startY) / 2.0f, -1.0f);
				Vector3 size = new Vector3((room.endX - room.startX), (room.endY - room.startY), 10.0f);
					
				// Eugh, make a gizmos wire-cube function...
				Gizmos.DrawWireCube(pos, size);
			}
		}
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(m_playerSpawnPoint + new Vector2(0.5f, 0.5f), 0.3f);
	
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(m_goalSpawnPoint + new Vector2(0.5f, 0.5f), 0.3f);	
	}
		
	public void OnGUI()
	{
		int sections = 0;
		foreach(var section in m_sections)
		{
			if(section.m_visibleSections > 0)
			{
				sections++;	
			}
		}
		
		GUI.Label(new Rect(0, 0, 300, 300), "Updating " + sections + " sections");	
		
		if(GUI.Button(new Rect(0, 400, 150, 30), "Enable node-view"))
		{
			SetNetworkViewEnabled(true);	
		}
		
		if(GUI.Button(new Rect(0, 440, 150, 30), "Disable node-view"))
		{
			SetNetworkViewEnabled(false);	
		}
	}
	
	// Editor variables
	public bool m_renderColliders 	= false;
	public bool m_renderNodeGraph 	= false;
	public bool m_renderRooms		= false;
#endif
	
	public GameObject TileType;
	public int SectionCountX = 30;
	public int SectionCountY = 30;
	public int Seed = -1;
	
	public static int m_sectionSize = 30;
	
	private Route m_lastRoute = null;
	
	[SerializeField]
	public List<LevelSection> m_sections = new List<LevelSection>();
	
	[SerializeField]
	public GameObject m_npcObject = null;
	
	private const string s_sectionsID = "sections";
	
	[SerializeField]
	private int m_previousSectionCountX = 30;
	
	[SerializeField]
	private int m_previousSectionCountY = 30;
	
	[SerializeField]
	private VisibilityReporter m_visReporter = null;
	
	[SerializeField] 
	private AIGraph m_graph = null;
	
	[SerializeField]
	private List<Room> m_rooms = new List<Room>();
	
	private List<LevelObject> m_levelObjects = new List<LevelObject>();
	
	// TODO: All these exist to allow the level-generator function. 
	//		 They need to go somewhere more sensible.
	[SerializeField]
	private Vector2 m_playerSpawnPoint;
	
	[SerializeField]
	private Vector2 m_goalSpawnPoint;
	
	[SerializeField]
	private int m_npcCount = 1;
	
	[SerializeField]
	private Dictionary<int, NetworkViewID> m_levelObjectIDs = null;
	
	//////////////////////////////////////////////////////////////////////////////////
	// DEBUG
	//////////////////////////////////////////////////////////////////////////////////
	public List<AreaPartition> RoomAreas = null;
	public List<Corridor> Corridors = null;
	
}
