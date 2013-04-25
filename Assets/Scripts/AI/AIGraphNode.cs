using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AIGraphNode
{
	public Vector2 NodePosition 
	{ 
		get { return m_nodePosition; }
		set { m_nodePosition = value; }
	}
	public List<AIGraphNode> NodeLinks
	{
		get { return m_nodeLinks; }	
		set { m_nodeLinks = value; }
	}
	
	public AIGraphNode() 
	{
	}
	
	private Vector2 m_nodePosition = new Vector2();
	
	private List<AIGraphNode> m_nodeLinks = new List<AIGraphNode>();
}
