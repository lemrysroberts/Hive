/// <summary>
/// AI graph.
/// 
/// Much like the RouteFinder, this class was left to grow up on the streets and has no respect for authority.
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AIGraph
{
	public AIGraphNode[] Nodes 
	{ 
		get { return m_nodes; }
		set { m_nodes = value; }
	}
	
	public AIGraph(int levelWidth, int levelHeight)
	{
		MaxIndex = 0;
		m_nodes = new AIGraphNode[levelWidth * levelHeight];
		m_width = levelWidth;
		m_height = levelHeight;
	}
	
	// TODO: 	This is awful, come up with some sort of spatial representation.
	// 			Also, make that spatial representation an area representation (i.e. navmesh). Ta.
	public AIGraphNode GetNearestNodeToPosition(Vector2 position)
	{
		AIGraphNode currentClosest = null;
		float currentMinDistance = float.MaxValue;
		
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
	
	public void Add(AIGraphNode node)
	{
		int x = (int)node.NodePosition.x;
		int y = (int)node.NodePosition.y;
		if(x > m_width || y > m_height)
		{
			return;
		}
	
		node.ID = x * m_height + y;
		m_nodes[x * m_height + y] = node;
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
	
	public static int MaxIndex = 0;
	private AIGraphNode[] m_nodes;
	private int m_width;
	private int m_height;
}
