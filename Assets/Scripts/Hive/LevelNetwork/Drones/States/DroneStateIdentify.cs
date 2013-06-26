///////////////////////////////////////////////////////////
// 
// DroneStateRoute.cs
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

public class DroneStateIdentify : IDroneState
{
	public delegate LevelNetworkNode PositionProvider();
	
	public DroneStateIdentify(AdminDrone drone, NodeSessionClient client, ref NodeSession session)
	{
		m_drone		= drone;
		m_client 	= client;
		m_session 	= session;
	}
	
	public void Start()
	{
		m_session = m_drone.m_originNode.CreateSession(m_client);
			
		List<LevelNetworkCommand> commands =  m_drone.m_originNode.GetCommands(m_session);
		
		foreach(var command in commands)
		{
			if(command.Name == "identify")
			{
				m_request = m_session.IssueCommand(command);
			}
		}
	}
	
	public UpdateResult Update()
	{
		if(m_request == null)
		{
			return UpdateResult.Failed;
		}
		
		if(m_request.State == LevelNetworkCommandRequest.CommandState.Completed)
		{
			m_drone.m_originNode.SetIdentified();
			return UpdateResult.Complete;
		}
		
		return UpdateResult.Updating;
	}
	
	private AdminDrone m_drone 						= null;
	private NodeSessionClient m_client 				= null;
	private NodeSession m_session 					= null;
	private LevelNetworkCommandRequest m_request 	= null;
}
