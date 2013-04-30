using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AIGraph
{
	public Dictionary<int, AIGraphNode> Nodes 
	{ 
		get { return m_nodes; }
		set { m_nodes = value; }
	}
	
	public AIGraph()
	{
		MaxIndex = 0;
	}
	
	// TODO: 	This is awful, come up with some sort of spatial representation.
	// 			Also, make that spatial representation an area representation (i.e. navmesh). Ta.
	public AIGraphNode GetNearestNodeToPosition(Vector2 position)
	{
		AIGraphNode currentClosest = null;
		float currentMinDistance = float.MaxValue;
		
		foreach(var node in m_nodes)
		{
			float distance = (node.Value.NodePosition - position).sqrMagnitude;
			if(distance < currentMinDistance)
			{
				currentClosest = node.Value;
				currentMinDistance = distance;
			}
		}
		return currentClosest;
	}
	
	public AIGraphNode GetRandomNode()
	{
		int endPosID = (int)(UnityEngine.Random.value * (float)(m_nodes.Count - 1));	
		return m_nodes[endPosID];
	}
	
	public static int MaxIndex = 0;
	
	private Dictionary<int, AIGraphNode> m_nodes = new Dictionary<int, AIGraphNode>();
}
