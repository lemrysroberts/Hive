///////////////////////////////////////////////////////////
// 
// LevelNetwork.cs
//
// What it does: Contains all the nodes that make up the level-network, as well as an AI-Graph that matches them.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelNetwork 
{
	public delegate void NodeAddedHandler(LevelNetworkNode newNode);
	
	public event NodeAddedHandler NodeAdded;
	
	public void Reset()
	{
		m_nodes.Clear();	
	}
	
	public void AddNode(LevelNetworkNode node)
	{
		m_nodes.Add(node);
		
		AIGraphNode newAINode =	m_networkGraph.AddNode(node.transform.position);
		newAINode.NodeObject = node;
		
		node.AINode = newAINode;
		
		if(NodeAdded != null)
		{
			NodeAdded(node);	
		}
	}
	
	public LevelNetwork()
	{
		Reset();	
	}
	
	public void BuildNodeConnections()
	{
		foreach(var node in m_nodes)
		{
			node.Reset();	
		}
		
		foreach(var node in m_nodes)
		{
			foreach(var id in node.ConnectionIDs)
			{
				var otherNode = GetNode(id);
				
				if(otherNode == null)
				{
					Debug.LogError("Failed to find node with ID: " + id);
					continue;
				}
				
				node.ConnectNode(otherNode);
			}
		}
	}
	
	// TODO: Maybe pre-build a dictionary to make this faster?
	// 		 Unity refuses to serialize Dictionaries at the moment, so it would have to
	//		 be done OnEnable().
	
	/// <summary>
	/// Gets a node from a given ID.
	/// Slow. As. Fuck.
	/// </summary>
	public LevelNetworkNode GetNode(int nodeID)
	{
		foreach(var node in m_nodes)
		{
			if(node.ID == nodeID)
			{
				return node;	
			}
		}
		
		return null;
	}
	
	public List<LevelNetworkNode> Nodes
	{
		get { return m_nodes; }
		set { m_nodes = value; }
	}
	
	public RouteFinder RouteFinder
	{
		get { return m_routeFinder; }
	}
	
	public AIGraph RouteGraph
	{
		get { return m_networkGraph; }		
	}		
	
	[SerializeField]
	private List<LevelNetworkNode> m_nodes = new List<LevelNetworkNode>();
	
	private AIGraph m_networkGraph = new AIGraph();
	private RouteFinder m_routeFinder = new RouteFinder();
}
