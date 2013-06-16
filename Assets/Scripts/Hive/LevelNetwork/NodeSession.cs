///////////////////////////////////////////////////////////
// 
// NodeSession.cs
//
// What it does: Tracks permissions for a given interaction with a node.
//
// Notes:
// 
// To-do: I'm tempted to make yet another class to avoid having explicit AdminDrone references in here.
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

public class NodeSession
{
	public NodeSession(LevelNetworkNode node, NodeSessionClient client)
	{
		m_node = node;
		m_client = client;
		
		m_node.LogActivity(this, "Session Started");
	}
	
	public void EndSession()
	{
		m_node.LogActivity(this, "Session Ended");
	}
	
	public NodeSessionClient Client
	{
		get { return m_client; }
	}
	
	public LevelNetworkNode Node
	{
		get { return m_node; }	
	}
	
	public int PermissionsLevel
	{
		get { return m_permissionsLevel; }
	}
	
	private int m_permissionsLevel = 0;
	private LevelNetworkNode m_node = null;
	private NodeSessionClient m_client = null;
}