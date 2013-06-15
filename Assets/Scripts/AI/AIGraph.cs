//////////////////////////////////////////////////////////////
// 
// AIGraph.cs
//
// What it does: 
//
// Notes: Much like the RouteFinder, this class was left to grow up on the streets and has no respect for authority.
// 
// To-do: I would quite like to re-use this for pathfinding through the LevelNetwork as well, but the binding to the Level
//		  class is likely to make that tricky.
//		  Why have I used an array the same size as the level that's half-filled with nulls? What were you doing, past-Luke...
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

public class AIGraph
{
	public List<AIGraphNode> Nodes 
	{ 
		get { return m_nodes; }
		set { m_nodes = value; }
	}
	
	public AIGraph()
	{
		m_nodes = new List<AIGraphNode>();
	}
	
	public void Reset()
	{
		m_nodes.Clear();	
	}
	
	public AIGraphNode AddNode(Vector2 position)
	{
		AIGraphNode newNode 	= new AIGraphNode();
		
		newNode.ID = m_nodes.Count;
		newNode.NodePosition = position;
		
		m_nodes.Add(newNode);
		
		return newNode;
	}
	
	private List<AIGraphNode> m_nodes;
}
