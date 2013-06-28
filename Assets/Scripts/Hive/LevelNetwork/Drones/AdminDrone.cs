///////////////////////////////////////////////////////////
// 
// AdminDrone.cs
//
// What it does: 	Base class for an admin-controlled drone.
//					Handles issuing of commands.
//					Controls the state-machine for each drone.
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
	public virtual event System.Action Activated;
	public virtual event System.Action Deactivated;
	
	public abstract List<LevelNetworkCommand> GetCommands();
	public abstract List<string> GetInfo(bool getNodeInfo);
	protected abstract void StartInternal();
	
	public bool SupportsSelection 	{	get { return m_supportsSelection; }	}
	public bool Highlighted			{	get { return m_highlighted; } }
	
	public AdminDrone()
	{
		m_session 					= new NodeSession();
		m_sessionClient 			= new NodeSessionClient();
		m_sessionClient.ClientName 	= "User1134"; // TODO: pointless	
	}
	
	public void DroneGUI()
	{
		if(m_currentState != null)
		{
			m_currentState.OnGUI();	
		}
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
	
	public void Start()
	{
		StartInternal();
		ResetState();
	}
	
	public void FixedUpdate()
	{
		if(m_currentState != null)
		{
			UpdateResult result = m_currentState.Update();
			
			switch(result)
			{
				case UpdateResult.Updating: break;
				case UpdateResult.Complete:
				{
					if(m_currentStateIndex < m_states.Count - 1)
					{
						m_currentStateIndex++;
						m_currentState = m_states[m_currentStateIndex];
						m_currentState.Start();
					}
					else
					{
						ResetState();	
					}
					break;
				}
				case UpdateResult.Failed: { ResetState(); break; }
			}
		}
	}
	
	private void ResetState()
	{
		m_currentStateIndex = 0;
			
		if(m_states.Count > 0)
		{
			m_currentState = m_states[0];
			m_currentState.Start();
		}
	}
	
	public Color 				defaultColor 		= Color.white;
	public LevelNetworkNode 	m_originNode 		= null;
	public LevelNetworkNode 	m_destinationNode 	= null;
	public NodeSessionClient 	m_sessionClient		= null;
	public NodeSession 			m_session			= null;
	public LevelNetworkNode 	m_targetNode		= null;
	
	protected LevelNetwork 		m_network 			= null;
	protected Level 			m_level 			= null;
	protected float 			m_progress 			= 0.0f;
	protected float 			m_lerpSpeed 		= 0.0f;
	protected bool 				m_supportsSelection	= true;
	protected bool 				m_highlighted		= false;
	protected List<IDroneState> m_states			= new List<IDroneState>();
	protected IDroneState 		m_currentState		= null;
	protected int 				m_currentStateIndex	= 0;
}
