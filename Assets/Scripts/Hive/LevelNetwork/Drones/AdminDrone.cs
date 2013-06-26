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
	public delegate void StateChangedHandler(bool newValue);
	
	public virtual event System.Action Deselected;
	public virtual event System.Action Selected;
	public virtual event StateChangedHandler HighLightStateChanged;
	public abstract event System.Action Activated;
	public abstract event System.Action Deactivated;
	
	public abstract List<LevelNetworkCommand> GetCommands();
	public abstract List<string> GetInfo(bool getNodeInfo);
	
	public bool SupportsSelection
	{
		get { return m_supportsSelection; }	
	}
	
	public bool Highlighted
	{
		get { return m_highlighted; }	
	}
	
	public void SetSelected(bool selected)
	{
		if(selected && Selected != null)
		{
			Selected();
		}
		else if(!selected && Deselected != null)
		{
			Deselected();	
		}
	}
	
	public void HighlightGained()
	{
		m_highlighted = true;
		
		if(HighLightStateChanged != null)
		{
			HighLightStateChanged(true);
		}	
	}
	
	public void HighlightLost()
	{
		m_highlighted = false;
		
		if(HighLightStateChanged != null)
		{
			HighLightStateChanged(false);
		}	
	}
	
	public virtual float GetResourceUsage()
	{
		return 0.0f;	
	}
	
	public Color defaultColor = Color.white;
	
	protected LevelNetwork m_network 				= null;
	protected Level m_level 						= null;
	protected LevelNetworkNode m_originNode 		= null;
	protected LevelNetworkNode m_destinationNode 	= null;
	protected float m_progress 						= 0.0f;
	protected float m_lerpSpeed 					= 0.0f;
	protected bool m_supportsSelection				= true;
	protected bool m_highlighted					= false;
}
