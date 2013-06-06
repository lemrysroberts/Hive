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
		
		
		if(NodeAdded != null)
		{
			NodeAdded(node);	
		}
	}
	
	public LevelNetwork()
	{
		Reset();	
	}
	
	public void RebuildNodeConnections()
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
	
	[SerializeField]
	private List<LevelNetworkNode> m_nodes = new List<LevelNetworkNode>();
}
