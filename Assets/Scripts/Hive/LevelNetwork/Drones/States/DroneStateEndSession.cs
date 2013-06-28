///////////////////////////////////////////////////////////
// 
// DroneStateEndSession.cs
//
// What it does: Begins a session at the current node.
//
// Notes: 
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class DroneStateEndSession : IDroneState
{
	public delegate LevelNetworkNode PositionProvider();
	
	public DroneStateEndSession(AdminDrone drone, NodeSessionClient client, ref NodeSession session)
	{
		m_drone		= drone;
		m_client 	= client;
		m_session 	= session;
	}
	
	public void Start()
	{
		m_drone.m_originNode.EndSession(m_session);
	}
	
	public UpdateResult Update()
	{
		return UpdateResult.Complete;	
	}
	
	public string GetStateInfo()
	{
		return m_stateString;
	}
	
	public void	OnGUI() { }
	
	private AdminDrone m_drone 						= null;
	private NodeSessionClient m_client 				= null;
	private NodeSession m_session 					= null;
	private string m_stateString					= string.Empty;
}
