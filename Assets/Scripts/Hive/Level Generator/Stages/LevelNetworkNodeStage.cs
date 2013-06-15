/// <summary>
/// Gathers all LevelNetworkNode scripts and registers them with the LevelNetwork, then creates GameObjects for the nodes.
/// 
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


public class LevelNetworkNodeStage : IGeneratorStage
{
	public LevelNetworkNodeStage(Level level)
	{
		m_level = level;	
	}
	
	public void Start()
	{
		m_level.Network.Reset();
		m_database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		m_prefab = m_database.GetEditorObject(m_networkNodeDatabaseID);
		m_connectionPrefab = m_database.GetEditorObject(m_networkConnectionDatabaseID);
	}
	
	public void End()
	{
		m_level.Network.BuildNodeConnections();
	}
	
	public void UpdateStep()
	{
		LevelNetwork network = m_level.Network;
		network.Reset();
		
		// Grab the "Network" child from the "Level" GameObject.
		GameObject networkObject = m_level.GetNetworkParentObject(false);
		
		// Create an empty list for the triangulation.
		List<Triangulator.Geometry.Point> points = new List<Triangulator.Geometry.Point>();
		
		MaxNodeID = 0;
		
		// Find all the nodes in the level.
		Object[] nodes = GameObject.FindObjectsOfType(typeof(LevelNetworkNode));
		List<LevelNetworkNode> graphNodes = new List<LevelNetworkNode>();
		
		foreach(var node in nodes)
		{
			LevelNetworkNode networkNode = node as LevelNetworkNode;
			network.AddNode(networkNode);
			networkNode.SetID(MaxNodeID);
			MaxNodeID++;
			
			if(GameFlow.Instance.View == WorldView.Admin && m_prefab != null)
			{
				GameObject newObject = GameObject.Instantiate(m_prefab) as GameObject;
				newObject.transform.parent = networkObject.transform;
				
				Vector3 newPosition = networkNode.transform.position;
				newPosition.z = networkObject.transform.position.z;
				newObject.transform.position = newPosition;
				
				
				LevelNetworkSelectableNode selectableNode = newObject.GetComponent<LevelNetworkSelectableNode>();
				if(selectableNode != null)
				{
					selectableNode.m_node = networkNode;	
				}
				else
				{
					Debug.LogWarning("AdminView network-node prefab does not have a NetworkSelectableNode component");
				}
			}
			
			// Only add nodes to the graph if they have no custom-connection specified.
			// This prevents special nodes being linked up.
			if(networkNode.CustomConnection == null)
			{
				points.Add(new Triangulator.Geometry.Point(networkNode.transform.position.x, networkNode.transform.position.y));
				graphNodes.Add(networkNode);
			}
		}
		
		if(GameFlow.Instance.View == WorldView.Admin)
		{
			try
			{
				List<Triangulator.Geometry.Triangle> tris = Triangulator.Delauney.Triangulate(points);	
				
				foreach(var tri in tris)
				{
					graphNodes[tri.p1].ConnectNode(graphNodes[tri.p2]);
					graphNodes[tri.p1].ConnectNode(graphNodes[tri.p3]);
				}
			}
			catch(System.Exception e)
			{
				Debug.LogError("Error calculating Delauney edges: " + e);
			}
			
			// Wire up any custom connections
			foreach(var node in network.Nodes)
			{
				if(node.CustomConnection != null)
				{
					node.ConnectNode(node.CustomConnection);	
				}
			}
			
			// TODO: LineRenderers are currently used by the connection prefab. These aint batched, which be shit.
			foreach(var node in network.Nodes)
			{
				foreach(var connection in node.Connections)
				{
					if(!connection.gameObjectCreated)
					{
						GameObject connectionObject = GameObject.Instantiate(m_connectionPrefab) as GameObject;
						connectionObject.transform.parent = networkObject.transform;
						connectionObject.transform.position = networkObject.transform.position;
						
						LevelNetworkConnectionRenderer connectionRenderer = connectionObject.GetComponent<LevelNetworkConnectionRenderer>();
						if(connectionRenderer != null)
						{
							connectionRenderer.m_connection = connection;
						}
						else
						{
							Debug.LogError("Level-network connection prefab does not have a \"LevelNetworkConnectionRenderer\" component");	
						}
						
						LineRenderer renderer = connectionObject.GetComponent<LineRenderer>();
						if(renderer != null)
						{
							renderer.SetVertexCount(2);
							Vector3 v0 = connection.startNode.transform.position;
							Vector3 v1 = connection.endNode.transform.position;
							
							v0.z = networkObject.transform.position.z;
							v1.z = networkObject.transform.position.z;
							
							renderer.SetPosition(0, v0);	
							renderer.SetPosition(1, v1);
						}
					
						connection.gameObjectCreated = true;	
					}
				}
			}
		}
	}
	
	public void UpdateAll()
	{
		UpdateStep();
	}
	
	public bool StageComplete() { return true; }
	public void SetupGUI()
	{
#if UNITY_EDITOR
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, GetStageName());
		
		if(m_showFoldout)
		{
			if(m_database == null)
			{
				m_database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
				m_prefab = m_database.GetEditorObject(m_networkNodeDatabaseID);
				m_connectionPrefab = m_database.GetEditorObject(m_networkConnectionDatabaseID);
				m_portPrefab = m_database.GetEditorObject(m_networkPortDatabaseID);
			}
			
			GUILayout.BeginVertical();
			
			var prefab = EditorGUILayout.ObjectField(m_prefab, typeof(GameObject), false) as GameObject;
			var connectionPrefab = EditorGUILayout.ObjectField(m_connectionPrefab, typeof(GameObject), false) as GameObject;
			var portPrefab = EditorGUILayout.ObjectField(m_portPrefab, typeof(GameObject), false) as GameObject;
						
			GUILayout.EndVertical();
			
			if(prefab != m_prefab)
			{
				m_database.AddEditorObject(m_networkNodeDatabaseID, prefab);
				m_prefab = prefab;
			}
			
			if(connectionPrefab != m_connectionPrefab)
			{
				m_database.AddEditorObject(m_networkConnectionDatabaseID, connectionPrefab);
				m_connectionPrefab = connectionPrefab;
			}
			
			if(portPrefab != m_portPrefab)
			{
				m_database.AddEditorObject(m_networkPortDatabaseID, portPrefab);
				m_portPrefab = portPrefab;
			}
		}
#endif
	}
	
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Create Network"; }
	
	public static int MaxNodeID = -1;
	
	private Level m_level;
	private static bool m_showFoldout = false;
	private static GameObject m_prefab = null;
	private static GameObject m_connectionPrefab = null;
	private static GameObject m_portPrefab = null;
	private LevelObjectDatabase m_database = null;
	private const string m_networkNodeDatabaseID 		= "networknodeprefab";
	private const string m_networkConnectionDatabaseID 	= "networkconnectionprefab";
	private const string m_networkPortDatabaseID 		= "networkportprefab";
}
