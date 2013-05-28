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
	
	private LevelNetwork()
	{
		Reset();	
	}
	
	public static LevelNetwork Instance
	{
		get 
		{
			if(s_instance == null)
			{
				s_instance = new LevelNetwork();	
			}
			
			return s_instance;
		}
	}
	
	public List<LevelNetworkNode> Nodes
	{
		get { return m_nodes; }
		set { m_nodes = value; }
	}
	
	[SerializeField]
	private List<LevelNetworkNode> m_nodes = new List<LevelNetworkNode>();
	
	private static LevelNetwork s_instance = null;
}
