///////////////////////////////////////////////////////////
// 
// DroneStateWaitForTarget.cs
//
// What it does: Just waits until m_target is not null.
//
// Notes: 
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class DroneStateWaitForTarget : IDroneState
{
	public DroneStateWaitForTarget(AdminDrone drone)
	{
		m_drone		= drone;
	}
	
	public void Start()	{ }
	
	public UpdateResult Update()
	{
		if(m_drone.m_targetNode == null)
		{
			m_stateString = "Waiting for target";
			return UpdateResult.Updating;
		}
		
		return UpdateResult.Complete;
	}
	
	public string GetStateInfo()
	{
		return m_stateString;
	}
	
	public void	OnGUI() { }
	
	private AdminDrone m_drone 						= null;
	private string m_stateString					= string.Empty;
}
