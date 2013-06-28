///////////////////////////////////////////////////////////
// 
// DroneStateWaitForCommand.cs
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

public class DroneStateWaitForCommand : IDroneState
{
	public DroneStateWaitForCommand(AdminDrone drone)
	{
		m_drone	= drone;
	}
	
	public void Start()	{ }
	
	public UpdateResult Update()
	{
		if(!m_progress)
		{
			m_stateString = "Waiting for command";
			return UpdateResult.Updating;
		}
		return UpdateResult.Complete;
	}
	
	public string GetStateInfo()
	{
		return m_stateString;
	}
	
	public void	OnGUI() 
	{
		{
			m_progress = true;	
		}
	}
	
	private AdminDrone m_drone 		= null;
	private string m_stateString	= string.Empty;
	private bool m_progress = false;
}
