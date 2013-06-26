///////////////////////////////////////////////////////////
// 
// LevelNetworkNode.cs
//
// What it does: Represents a given node in the level-network.
//
// Notes: This has been a hub for sin, so double-check everything.
// 
// To-do:
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public class LevelNetworkNode : MonoBehaviour 
{
	void Start () 
	{
		m_identified 		= false;
		Claimant 			= null;
		Heat 				= 0.0f;
		ActivityProgress 	= 0.0f;
		ActivityInProgress 	= false;
	}
	
	void FixedUpdate () 
	{
		Heat -= HeatCooldown;
		Heat = Mathf.Max(0.0f, Heat);
		
		List<LevelNetworkCommandRequest> completedRequests = new List<LevelNetworkCommandRequest>();
		
		foreach(var request in m_activeRequests)
		{
			Heat += request.Command.UpdateHeat;
			request.UpdateProgress(Time.deltaTime);
			
			
			if(request.State == LevelNetworkCommandRequest.CommandState.Completed)
			{
				IssueCommand(request.Session, request.Command);
				completedRequests.Add(request);
			}
		}
		
		// Remove completed requests
		foreach(var request in completedRequests)
		{
			m_activeRequests.Remove(request);	
		}
	}
	
	public void SetID(int id)
	{
		m_ID = id;	
	}
	
	public void ConnectNode(LevelNetworkNode other)
	{
		if(m_connectedNodes.Contains(other) || other.ConnectedNodes.Contains(this) || other == this)
		{
			return;	
		}
		
		if(!m_connectionIDs.Contains(other.ID))
		{
			m_connectionIDs.Add(other.ID);
		}
		ConnectedNodes.Add(other);
		other.ConnectedNodes.Add(this);
		
		LevelNetworkConnection newConnection = new LevelNetworkConnection();
		newConnection.startNode = this;
		newConnection.endNode = other;
		
		m_connections.Add(newConnection);
		other.Connections.Add(newConnection);
		
		// TODO:  I have a horrible suspicion that this may cause leaks.
		// DANGER, DANGER
		AINode.NodeLinks.Add(other.AINode);
		other.AINode.NodeLinks.Add(AINode);
	}
	
	public List<LevelNetworkCommand> GetCommands(NodeSession session)
	{
		if(target == null)
		{
			Debug.LogError("No Target set");
			return null;
		}
		
		List<LevelNetworkCommand> commands = target.GetCommands(session.PermissionsLevel);
		
		commands.Add(new LevelNetworkCommand("identify", "Identify", 3.0f, 0.001f));
		commands.Add(new LevelNetworkCommand("hack_clearance", "Hack Clearance Level", 15.0f, 0.002f));
		
		return commands;
	}
	
	public List<string> InfoStrings
	{
		get 
		{ 
			List<string> infoStrings = target.GetInfoStrings(); 
			infoStrings.Add("Test");
			
			return infoStrings;
		}	
	}
	
	public LevelNetworkCommandRequest RequestCommand(NodeSession session, LevelNetworkCommand command)
	{
		string logString = session.Client.ClientName + ": [" + command.Name + "]";
		m_activityLog.Add(logString);
		
		LevelNetworkCommandRequest newRequest = new LevelNetworkCommandRequest(session, command);	
		m_activeRequests.Add(newRequest);
		
		return newRequest;
	} 
	
	public void IssueCommand(NodeSession session, LevelNetworkCommand command)
	{
		if(!CheckDefaultCommands(session, command))
		{
			target.IssueCommand(command);	
		}
	}
	
	// This function is probably overly thorough, but rather that than leaving dangling connections.
	public void Reset()
	{
		foreach(var node in m_connectedNodes)
		{
			node.ConnectedNodes.Remove(this);
		}
		ConnectedNodes.Clear();
		
		foreach(var connection in m_connections)
		{
			var otherNode = connection.startNode == this ? connection.endNode : connection.startNode;
			otherNode.Connections.Remove(connection);
		}
		
		m_connections.Clear();
	}
	
	public NodeSession CreateSession(NodeSessionClient client)
	{
		NodeSession newSession = new NodeSession(this, client);
		m_activeSessions.Add(newSession);
		
		return newSession;
	}
	
	public void EndSession(NodeSession session)
	{
		session.EndSession();
		m_activeSessions.Remove(session);	
	}
	
	public void LogActivity(NodeSession session, string activity)
	{
		string logEntry  = session.Client.ClientName + ": " + activity;
		m_activityLog.Add(logEntry);
	}
	
	public void SetIdentified()
	{
		m_identified = true;
		if(NodeIdentified != null)
		{
			NodeIdentified(this);
		}
	}
	
	#region Properties
	public List<LevelNetworkNode> ConnectedNodes
	{
		get { return m_connectedNodes; }
		set { m_connectedNodes = value; }
	}
	
	private bool CheckDefaultCommands(NodeSession session, LevelNetworkCommand command)
	{
		if(command.Name == "hack_clearance")
		{
			session.PermissionsLevel++;
		}
		
		return false;
	}
	
	public List<string> ActivityLog
	{
		get { return m_activityLog; }	
	}
	
	public List<LevelNetworkConnection> Connections
	{
		get { return m_connections; }	
	}
	
	public List<int> ConnectionIDs
	{
		get { return m_connectionIDs; }	
	}
	
	public int ID
	{
		get { return m_ID; }	
	}
	
	public float Heat
	{
		get; set;	
	}
	
	public float ActivityProgress
	{
		get; set;	
	}
	
	public bool ActivityInProgress
	{
		get; set;	
	}
	
	public AIGraphNode AINode
	{
		get; set;	
	}
	
	public bool Identified
	{
		get { return m_identified; }
	}
	
	public AdminDrone Claimant
	{
		get; set;
	}
	#endregion
	
	#region Fields
	public delegate void NodeIdentifiedHandler(LevelNetworkNode node);
	public event NodeIdentifiedHandler NodeIdentified;
	
	public Texture2D NodeTexture 								= null;
	public Texture2D IdentifiedTexture							= null;
	public LevelNetworkNode CustomConnection 					= null;
	public LevelNetworkCommandIssuer target 					= null;
	public float HeatCooldown 									= 0.0001f;
	public float ClaimSpeed 									= 0.1f;
	public bool ConnectedToPort 								= false;
	
	private bool m_identified 									= false;
	private int m_ID 											= -1;
	private List<int> m_connectionIDs 							= new List<int>();
	private List<LevelNetworkNode> m_connectedNodes 			= new List<LevelNetworkNode>();
	private List<LevelNetworkConnection> m_connections 			= new List<LevelNetworkConnection>();
	private List<NodeSession> m_activeSessions					= new List<NodeSession>();
	private List<LevelNetworkCommandRequest> m_activeRequests 	= new List<LevelNetworkCommandRequest>();
	private List<string> m_activityLog 							= new List<string>();
	#endregion
}
