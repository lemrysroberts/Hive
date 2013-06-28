///////////////////////////////////////////////////////////
// 
// HackDrone.cs
//
// What it does: Moves from node-to-node using a simple AI state-machine.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class HackDrone : AdminDrone 
{
	public bool DrawRoute = false;
	
	public override event System.Action Activated;
	public override event System.Action Deactivated;
	
	public HackDrone()
	{
		DroneStateWaitForTarget waitState			= new DroneStateWaitForTarget(this);
		DroneStateRoute routeState 					= new DroneStateRoute(this, new DroneStateRoute.PositionProvider(GetRouteDestination), 4.5f);
		DroneStateStartSession startSessionState	= new DroneStateStartSession(this, m_sessionClient, ref m_session);
		DroneStateIdentify identifyState 			= new DroneStateIdentify(this, m_sessionClient, ref m_session);
		DroneStateHack hackState					= new DroneStateHack(this, m_sessionClient, ref m_session);
		DroneStateEndSession endSessionState		= new DroneStateEndSession(this, m_sessionClient, ref m_session);
		DroneStateWaitForCommand commandState		= new DroneStateWaitForCommand(this);
		
		identifyState.Activated 	+= new System.Action(HandleActivated);
		identifyState.Deactivated 	+= new System.Action(HandleDeactivated);
		
		hackState.Activated 	+= new System.Action(HandleActivated);
		hackState.Deactivated 	+= new System.Action(HandleDeactivated);
		
		m_states.Add(waitState);
		m_states.Add(routeState);
		m_states.Add(startSessionState);
		m_states.Add(identifyState);	
		m_states.Add(hackState);	
		m_states.Add(commandState);
		m_states.Add(endSessionState);
	}
	
	// Use this for initialization
	protected override void StartInternal () 
	{
		m_level = FindObjectOfType(typeof(Level)) as Level;
		m_network = m_level.Network;
		
		LevelNetworkNode startNode = m_network.Nodes[0];
		transform.position = new Vector3(startNode.transform.position.x, startNode.transform.position.y, transform.position.z);
		
		m_originNode = startNode;
		
		m_sessionClient = new NodeSessionClient();
		m_sessionClient.ClientName = "User1134";
	}
	
	LevelNetworkNode GetRouteDestination()
	{
		return m_targetNode;	
	}
	
	public override List<string> GetInfo(bool getNodeInfo)
	{
		List<string> info = new List<string>();
		
		if(m_currentState != null)
		{
			info.Add(m_currentState.GetStateInfo());
		}
		
		if(getNodeInfo)
		{
			foreach(var logEntry in m_originNode.ActivityLog)
			{
				info.Add(logEntry);	
			}
		}
		
		return info;
	}
	
	public override List<LevelNetworkCommand> GetCommands()
	{
		return m_originNode.GetCommands(m_session);
	}
	
	private void HandleActivated()
	{
		if(Activated != null) { Activated(); }	
	}
	
	private void HandleDeactivated()
	{
		if(Deactivated != null) { Deactivated(); }	
	}
}
