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
	
	public int ID 
	{ 
		get { return m_ID; }
		set { m_ID = value; }
	}
	
	// Miscellaneous reference to identify associated objects when traversing a route.
	public System.Object NodeObject
	{
		get; set;	
	}
	
	private int m_ID = -1;
	private Vector2 m_nodePosition = new Vector2();
	
	private List<AIGraphNode> m_nodeLinks = new List<AIGraphNode>();
}
