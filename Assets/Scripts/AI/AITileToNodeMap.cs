///////////////////////////////////////////////////////////
// 
// AITileToNodeMap.cs
//
// What it does: 	By default, AI-graphs do not have helper functions to locate their nodes spatially.
//					This class maps the Level's tile-based structure onto an AI-graph for ease of access.
//
// Notes: Probably a bit wasteful, but it stops the AI being tied to the level so I can re-use it for the level-network.
// 
// To-do:
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

public class AITileToNodeMap
{
	
	public AITileToNodeMap(int levelWidth, int levelHeight)
	{
		m_graph = new AIGraph();
		
		m_nodes 	= new AIGraphNode[levelWidth * levelHeight];
		m_width 	= levelWidth;
		m_height 	= levelHeight;
	}
	
	// TODO: 	This is awful, come up with some sort of spatial representation.
	// 			Also, make that spatial representation an area representation (i.e. navmesh). Ta.
	public AIGraphNode GetNearestNodeToPosition(Vector2 position)
	{
		AIGraphNode currentClosest = null;
		float currentMinDistance = float.MaxValue;
		
		// TOD: Har, this is thick. 
		foreach(var node in m_nodes)
		{
			if(node != null)
			{
				float distance = (node.NodePosition - position).sqrMagnitude;
				if(distance < currentMinDistance)
				{
					currentClosest = node;
					currentMinDistance = distance;
				}
			}
		}
		return currentClosest;
	}
	
	public AIGraphNode GetNode(Vector2 position)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		if(x > m_width || y > m_height)
		{
			return null;	
		}
		
		return m_nodes[x * m_height + y];
	}
	
	public void AddNode(Vector2 position)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		if(x > m_width || y > m_height)
		{
			return;
		}
	
		AIGraphNode newNode = m_graph.AddNode(position);
		
		m_nodes[x * m_height + y] = newNode;
	}
	
	public int GetNodeIndex(Vector2 position)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		if(x > m_width || y > m_height)
		{
			return -1;	
		}
		
		return x * m_height + y;
	}
	
	public AIGraphNode GetRandomNode()
	{
		int bailout = 100;
		AIGraphNode randomNode = null;
		
		int count = 0;
		while(randomNode == null && count < bailout)
		{
			randomNode = m_nodes[(int)(UnityEngine.Random.value * (float)(m_nodes.Length - 1))];		
			count++;
		}
		
		
		return randomNode;
	}
	
	public AIGraph Graph
	{
		get { return m_graph; }	
	}
	
	public AIGraphNode[] TileMappedNodes
	{
		get { return m_nodes; }	
	}
	
	private AIGraph 		m_graph;
	private AIGraphNode[] 	m_nodes;
	private int 			m_width;
	private int 			m_height;
}