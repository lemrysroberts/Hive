using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public class LevelNetworkNode : MonoBehaviour 
{
	public LevelNetworkCommandIssuer target;
	
	public float HeatCooldown = 0.001f;
	public float ClaimSpeed = 0.002f;
	
	public delegate void NodeStateChangedHandler();
	public event NodeStateChangedHandler NodeClaimed;
	
	public bool ConnectedToPort = false;
	public LevelNetworkNode CustomConnection = null;
	
	public Texture2D NodeTexture = null;
	
	void Start () 
	{
		Heat = 0.0f;
		ClaimProgress = 0.0f;
		ClaimInProgress = false;
		Claimed = false;
	}
	
	void FixedUpdate () 
	{
		Heat -= HeatCooldown;
		Heat = Mathf.Max(0.0f, Heat);
		
		if(ClaimInProgress)
		{
			ClaimProgress += ClaimSpeed;
			if(ClaimProgress >= 1.0f)
			{
				Claimed = true;
				ClaimInProgress = false;
				if(NodeClaimed != null)
				{
					NodeClaimed();
				}
			}
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
	}
	
	public List<LevelNetworkCommand> Commands
	{
		get 
		{ 
			if(target == null)
			{
				Debug.LogError("No Target set");
				return null;
			}
			
			if(Claimed || !target.Claimable)
			{
				return target.GetCommandNames(); 
			}
			else
			{
				List<LevelNetworkCommand> functionNames = new List<LevelNetworkCommand>();
				
				if(ClaimInProgress)
				{
					functionNames.Add(new LevelNetworkCommand("cancel_claim", "Cancel Claim"));	
				}
				else
				{
					functionNames.Add(new LevelNetworkCommand("claim", "Claim (" + ClaimCost + ")"));	
				}
				
				return functionNames;
			}
			
		}	
	}
	
	public void IssueCommand(LevelNetworkCommand command)
	{
		Heat = 1.0f;
		if(!CheckDefaultCommands(command))
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
	
	private bool CheckDefaultCommands(LevelNetworkCommand command)
	{
		if(command.Name == "claim")
		{
			ClaimProgress = 0.0f;
			ClaimInProgress = true;
			return true;	
		}
		
		if(command.Name == "cancel_claim")
		{
			ClaimInProgress = false;
			return true;
		}
		
		return false;
	}
	
	public List<LevelNetworkNode> ConnectedNodes
	{
		get { return m_connectedNodes; }
		set { m_connectedNodes = value; }
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
	
	public float ClaimCost
	{
		get { return m_claimCost; }
	}
	
	public float ClaimProgress
	{
		get; set;	
	}
	
	public bool ClaimInProgress
	{
		get; set;	
	}
	
	public bool Claimed
	{
		get; set;	
	}
	
	[SerializeField]
	private List<int> m_connectionIDs = new List<int>();
	
	[SerializeField]
	private int m_ID = -1;
	
	[SerializeField]
	private float m_claimCost = 5.0f; // TODO: Think a bit about these arbitrary units.
	
	private float m_claimProgress = 0.0f;
	private List<LevelNetworkNode> m_connectedNodes = new List<LevelNetworkNode>();
	private List<LevelNetworkConnection> m_connections = new List<LevelNetworkConnection>();
	
}
