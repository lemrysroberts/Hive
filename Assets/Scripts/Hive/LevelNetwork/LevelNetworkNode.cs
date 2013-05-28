using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class LevelNetworkNode : MonoBehaviour 
{
	
	public LevelNetworkCommandIssuer target;
	
	void Start () 
	{
		
	}
	
	void Update () 
	{
	}
	

	
	public void ConnectNode(LevelNetworkNode other)
	{
		if(m_connectedNodes.Contains(other) || other.ConnectedNodes.Contains(this) || other == this)
		{
			return;	
		}
		
		ConnectedNodes.Add(other);
		other.ConnectedNodes.Add(this);
		
		LevelNetworkConnection newConnection = new LevelNetworkConnection();
		newConnection.startNode = this;
		newConnection.endNode = other;
		
		m_connections.Add(newConnection);
	}
	
	public List<string> FunctionNames
	{
		get { return target.GetCommandNames(); }	
	}
	
	public void IssueCommand(string commandName)
	{
		target.IssueCommand(commandName);
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
	
	private List<LevelNetworkNode> m_connectedNodes = new List<LevelNetworkNode>();
	private List<LevelNetworkConnection> m_connections = new List<LevelNetworkConnection>();
}
