///////////////////////////////////////////////////////////
// 
// AdminDrone.cs
//
// What it does: Handles interactions with terminals.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class AdminDrone : MonoBehaviour 
{
	protected LevelNetwork m_network 	= null;
	protected Level m_level 			= null;
	
	protected LevelNetworkNode m_originNode = null;
	protected LevelNetworkNode m_destinationNode = null;
	protected float m_progress = 0.0f;
	protected float m_lerpSpeed = 0.0f;
}
