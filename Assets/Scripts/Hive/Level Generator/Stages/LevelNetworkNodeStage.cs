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
		m_database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		m_prefab = m_database.GetEditorObject(m_networkNodeDatabaseID);
		m_connectionPrefab = m_database.GetEditorObject(m_networkConnectionDatabaseID);
	}
	
	public void End(){ }
	
	public void UpdateStep()
	{
		LevelNetwork network = LevelNetwork.Instance;
		network.Reset();
		
		GameObject networkObject = m_level.GetNetworkParentObject(true);
		
		List<Triangulator.Geometry.Point> points = new List<Triangulator.Geometry.Point>();
		
		Object[] nodes = GameObject.FindObjectsOfType(typeof(LevelNetworkNode));
		foreach(var node in nodes)
		{
			LevelNetworkNode networkNode = node as LevelNetworkNode;
			network.AddNode(networkNode);
			
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
			points.Add(new Triangulator.Geometry.Point(networkNode.transform.position.x, networkNode.transform.position.y));
		}
		
		if(GameFlow.Instance.View == WorldView.Admin)
		{
			try
			{
				List<Triangulator.Geometry.Triangle> tris = Triangulator.Delauney.Triangulate(points);	
				
				foreach(var tri in tris)
				{
					network.Nodes[tri.p1].ConnectNode(network.Nodes[tri.p2]);
					network.Nodes[tri.p1].ConnectNode(network.Nodes[tri.p3]);
				}
			}
			catch(System.Exception e)
			{
				Debug.LogError("Error calculating Delauney edges");
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
			}
			
			GUILayout.BeginVertical();
			
			var prefab = EditorGUILayout.ObjectField(m_prefab, typeof(GameObject), false) as GameObject;
			var connectionPrefab = EditorGUILayout.ObjectField(m_connectionPrefab, typeof(GameObject), false) as GameObject;
			
			
			
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
		}
#endif
	}
	
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Create Network"; }
	
	private Level m_level;
	private static bool m_showFoldout = false;
	private static GameObject m_prefab = null;
	private static GameObject m_connectionPrefab = null;
	private LevelObjectDatabase m_database = null;
	private const string m_networkNodeDatabaseID = "networknodeprefab";
	private const string m_networkConnectionDatabaseID = "networkconnectionprefab";
}
