using UnityEngine;
using System;
using System.Collections;
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
	}
	
	private List<AIGraphNode> m_nodes = new List<AIGraphNode>();
}
