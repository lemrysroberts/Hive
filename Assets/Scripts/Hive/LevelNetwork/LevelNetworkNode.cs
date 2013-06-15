using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public class LevelNetworkNode : MonoBehaviour 
{
	public LevelNetworkCommandIssuer target;
	
	public float HeatCooldown 	= 0.001f;
	public float ClaimSpeed 	= 0.1f;
	
	public event System.Action NodeClaimed;
	public event System.Action NodeRescinded;
	public event System.Action NodeAvailable;
	public event System.Action NodeUnavailable;
	
	public bool ConnectedToPort = false;
	public LevelNetworkNode CustomConnection = null;
	
	public Texture2D NodeTexture = null;
	
	void Start () 
	{
		Heat = 0.0f;
		ActivityProgress = 0.0f;
		ActivityInProgress = false;
		Claimed = false;
	}
	
	void FixedUpdate () 
	{
		Heat -= HeatCooldown;
		Heat = Mathf.Max(0.0f, Heat);
		
		if(ActivityInProgress)
		{
			ActivityProgress += ClaimSpeed;
			if(ActivityProgress >= 1.0f)
			{
				SetClaimed();
			}
		}
	}
	
	public void SetClaimed()
	{
		ActivityInProgress = false;
		Claimed = true;
		
		// Make sure other neighbours are set to be claimable
		foreach(var node in m_connectedNodes)
		{
			if(!m_claimedLinks.Contains(node))
			{
				node.m_claimedLinks.Add(this);
				node.UpdateState();
			}
		}
		
		if(NodeClaimed != null)
		{
			NodeClaimed();
		}
		
		
	}
	
	public void SetRescinded()
	{
		Claimed = false;
		foreach(var node in m_connectedNodes)
		{
			if(node.m_claimedLinks.Remove(this))
			{
				node.UpdateState();
			}
		}
		
		if(NodeRescinded != null)
		{
			NodeRescinded();
		}
	}
	
	public void SetID(int id)
	{
		m_ID = id;	
	}
	
	public void UpdateState()
	{
		if(m_claimedLinks.Count == 0)
		{
			SetRescinded();	
			
			if(NodeUnavailable != null)
			{
				NodeUnavailable();
			}
		}
		
		if(Claimed && NodeClaimed != null)
		{
			NodeClaimed();
		}
		else if(m_claimedLinks.Count > 0 && NodeAvailable != null)
		{
			NodeAvailable();
		}
	}
	
	public void AddLink(LevelNetworkNode other)
	{
		m_claimedLinks.Add(other);
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
				List<LevelNetworkCommand> targetCommands = target.GetCommands();
				
				targetCommands.Add(new LevelNetworkCommand("rescind_claim", "Rescind Claim"));
				
				targetCommands.Add(new LevelNetworkCommand("sdf", "Connections: " + m_claimedLinks.Count));
				
				return targetCommands; 
			}
			else
			{
				List<LevelNetworkCommand> functionNames = new List<LevelNetworkCommand>();
				
				if(ActivityInProgress)
				{
					functionNames.Add(new LevelNetworkCommand("cancel_claim", "Cancel Claim"));	
				}
				else if(m_claimedLinks.Count > 0)
				{
					functionNames.Add(new LevelNetworkCommand("claim", "Claim (" + ClaimCost + ")"));	
				}
				
				functionNames.Add(new LevelNetworkCommand("sdf", "Connections: " + m_claimedLinks.Count));
				return functionNames;
			}
			
		}	
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
			ActivityProgress = 0.0f;
			ActivityInProgress = true;
			return true;	
		}
		
		if(command.Name == "cancel_claim")
		{
			ActivityInProgress = false;
			return true;
		}
		
		if(command.Name == "rescind_claim")
		{
			SetRescinded();
			
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
	
	public bool Available
	{
		get { return m_claimedLinks.Count > 0 || Claimed; }	
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
	
	public float ActivityProgress
	{
		get; set;	
	}
	
	public bool ActivityInProgress
	{
		get; set;	
	}
	
	public bool Claimed
	{
		get; set;	
	}
	
	public AIGraphNode AINode
	{
		get; set;	
	}
	
	[SerializeField]
	private List<int> m_connectionIDs = new List<int>();
	
	[SerializeField]
	private int m_ID = -1;
	
	//[SerializeField]
	private float m_claimCost = 2.0f; // TODO: Think a bit about these arbitrary units.
	
	private List<LevelNetworkNode> m_connectedNodes = new List<LevelNetworkNode>();
	private List<LevelNetworkConnection> m_connections = new List<LevelNetworkConnection>();
	private List<LevelNetworkNode> m_claimedLinks = new List<LevelNetworkNode>();
}
