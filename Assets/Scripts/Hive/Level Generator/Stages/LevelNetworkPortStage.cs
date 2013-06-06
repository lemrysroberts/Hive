///////////////////////////////////////////////////////////
// 
// LevelNetworkPortStage.cs
//
// What it does: Places network-port objects around the level that connect to Terminal nodes.
//	
// How: It iterates in from the edges, checking terminals to see if they are within the current slice.
//		If they are, it adds the prefab specified in the editor and marks it as having a custom-connection,
//		to prevent it from being wired up to all surround nodes during the LevelNetworkNodeStage.
//
// Notes:
// 
// To-do: Randomise the order in which edges are searched to have a less predictable terminal layout.
//		  Currently it does all terminals from the left, then right, top, bottom.
//
///////////////////////////////////////////////////////////


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class LevelNetworkPortStage : IGeneratorStage
{
	public LevelNetworkPortStage(Level level)
	{
		m_level = level;	
	}
	
	public void Start()
	{
		m_level.Network.Reset();
		
		m_database 		= GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		m_portPrefab 	= m_database.GetEditorObject(m_networkPortDatabaseID);
		
		m_level.GetNetworkParentObject(true);
	}
	
	public void End(){ }
	
	public void UpdateStep()
	{
		// Grab the "Network" child from the "Level" GameObject.
		GameObject networkObject = m_level.GetNetworkParentObject(false);
		
		List<LevelNetworkNode> validNodes = new List<LevelNetworkNode>();
		
		Object[] nodes = GameObject.FindObjectsOfType(typeof(LevelNetworkNode));
		foreach(var node in nodes)
		{
			LevelNetworkNode currentNode = node as LevelNetworkNode;	
			if(!currentNode.ConnectedToPort && currentNode.GetComponent<Terminal>() != null)
			{
				validNodes.Add(currentNode);	
			}
		}
		
		// Search inwards for a valid terminal
		int searchDepth = 1;
		LevelNetworkNode candidateNode = null;
		PortEdge edge = PortEdge.Top;
		
		
		while(searchDepth < (m_level.Width / 2) && candidateNode == null)
		{
			foreach(var node in validNodes)
			{
				int nodeX = (int)node.gameObject.transform.position.x;
				int nodeY = (int)node.gameObject.transform.position.y;
				
				if( nodeX == searchDepth)
				{
					edge = PortEdge.Left;
					candidateNode = node;
					break;
				}
				
				if(nodeX == m_level.Width - searchDepth)
				{
					edge = PortEdge.Right;
					candidateNode = node;
					break;
				}
				
				if(nodeY == searchDepth )
				{
					edge = PortEdge.Bottom;
					candidateNode = node;
					break;	
				}
				
				if(nodeY == m_level.Height - searchDepth)
				{
					edge = PortEdge.Top;
					candidateNode = node;
					break;	
				}
			
			}
			
			searchDepth++;
		}
		
		if(candidateNode != null)
		{
			GameObject newObject = GameObject.Instantiate(m_portPrefab) as GameObject;	
			newObject.transform.parent = networkObject.transform;
			
			LevelNetworkNode node = newObject.GetComponent<LevelNetworkNode>();
			node.CustomConnection = candidateNode;
			
			
			switch(edge)
			{
				case PortEdge.Top: 		{ newObject.transform.position = new Vector3(candidateNode.transform.position.x, m_level.Height + 1.0f, 0.0f); break; }	
				case PortEdge.Bottom: 	{ newObject.transform.position = new Vector3(candidateNode.transform.position.x, - 1.0f, 0.0f); break; }
				case PortEdge.Left: 	{ newObject.transform.position = new Vector3( -1.0f, candidateNode.transform.position.y, 0.0f); break; }
				case PortEdge.Right: 	{ newObject.transform.position = new Vector3(m_level.Width + 1.0f, candidateNode.transform.position.y, 0.0f); break; }
			}
			candidateNode.ConnectedToPort = true;
			node.ConnectedToPort = true;
		}
		m_portAttempts++;
	}
	
	public void UpdateAll()
	{
		while(!StageComplete())
		{
			UpdateStep();
		}
	}
	
	public bool StageComplete() { return m_portAttempts >= m_portCount; }
	public void SetupGUI()
	{
#if UNITY_EDITOR
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, GetStageName());
		
		if(m_showFoldout)
		{
			if(m_database == null)
			{
				m_database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
				m_portPrefab = m_database.GetEditorObject(m_networkPortDatabaseID);
			}
			
			GUILayout.BeginVertical();
			
			var portPrefab = EditorGUILayout.ObjectField(m_portPrefab, typeof(GameObject), false) as GameObject;
						
			GUILayout.EndVertical();
			
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
	public string GetStageName(){ return "Create Network-Ports"; }
	
	private Level m_level;
	private static bool m_showFoldout = false;
	private static GameObject m_portPrefab = null;
	private LevelObjectDatabase m_database = null;
	private const string m_networkPortDatabaseID = "networkportprefab";
	private int m_portCount = 50;
	private int m_portAttempts = 0;
		
	/// <summary>
	/// Identifies which edge a port is connected from.
	/// </summary>
	private enum PortEdge
	{
		Top,
		Bottom,
		Left,
		Right
	}
}
