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
	public void StartSession(LevelNetworkNode node, NodeSessionClient client)
	{
		if(m_started)
		{
			Debug.LogError("Starting a new session before the previous has been ended.");	
		}
		
		m_node 		= node;
		m_client 	= client;
		m_started	= true;
		
		m_node.LogActivity(this, "Session Started");
	}
	
	public void EndSession()
	{
		m_node.LogActivity(this, "Session Ended");
		
		m_node 		= null;
		m_client 	= null;
		m_started	= false;
	}
	
	public LevelNetworkCommandRequest IssueCommand(LevelNetworkCommand command)
	{
		if(!m_started)
		{
			Debug.LogError("Cannot issue command \"" + command.Name + "\". Session not started.");
			return null;	
		}
		
		return m_node.RequestCommand(this, command);
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
		set { m_permissionsLevel = value; }
	}
	
	private int m_permissionsLevel 		= 0;
	private LevelNetworkNode m_node 	= null;
	private NodeSessionClient m_client 	= null;
	private bool m_started				= false;
}