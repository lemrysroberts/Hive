///////////////////////////////////////////////////////////
// 
// DroneStateHack.cs
//
// What it does: Identifies the drone with a node and creates a session.
//
// Notes: Probably has a strop if you return null from either provider function. So don't.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class DroneStateHack : IDroneState
{
	public event System.Action Activated;
	public event System.Action Deactivated;
	
	public DroneStateHack(AdminDrone drone, NodeSessionClient client, ref NodeSession session)
	{
		m_drone		= drone;
		m_client 	= client;
		m_session 	= session;
	}
	
	public void Start()
	{
		List<LevelNetworkCommand> commands =  m_drone.m_originNode.GetCommands(m_session);
			
		foreach(var command in commands)
		{
			if(command.Name == "hack_clearance")
			{
				m_request = m_session.IssueCommand(command);
				if(Activated != null)
				{
					Activated();	
				}
				
				return;
			}
		}
	}
	
	public UpdateResult Update()
	{
		m_stateString = "Testing Key: " + Random.Range(1, 8000);
		
		if(m_request.State == LevelNetworkCommandRequest.CommandState.Completed)
		{
			if(Deactivated != null)
			{
				Deactivated();	
			}
			
			return UpdateResult.Complete;
		}
		
		return UpdateResult.Updating;
	}
	
	public string GetStateInfo()
	{
		return m_stateString;
	}
	
	public void	OnGUI() { }
	
	private AdminDrone m_drone 						= null;
	private NodeSessionClient m_client 				= null;
	private NodeSession m_session 					= null;
	private LevelNetworkCommandRequest m_request 	= null;
	private string m_stateString					= string.Empty;
}
