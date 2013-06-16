///////////////////////////////////////////////////////////
// 
// AdminDrone.cs
//
// What it does: 	Base class for an admin-controlled drone.
//					Handles issuing of commands.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public abstract class AdminDrone : MonoBehaviour 
{
	public abstract List<LevelNetworkCommand> GetCommands();
	public abstract List<string> GetInfo();
	
	protected LevelNetwork m_network 	= null;
	protected Level m_level 			= null;
	
	protected LevelNetworkNode m_originNode = null;
	protected LevelNetworkNode m_destinationNode = null;
	protected float m_progress = 0.0f;
	protected float m_lerpSpeed = 0.0f;
}
