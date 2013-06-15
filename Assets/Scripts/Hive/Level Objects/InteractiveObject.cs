///////////////////////////////////////////////////////////
// 
// InteractiveObject.cs
//
// What it does: Base class defining a set of functions that allow the agent
//				 to interact with an object in the environment.
//				 Basically just offers a list of commands and handles them when selected.
//
// Notes: Roughly equivalent to the LevelNetworkCommandIssuer, but for the agent instead.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public abstract class InteractiveObject : MonoBehaviour 
{
	public abstract List<ObjectInteraction> GetInteractions();
	
}

public class ObjectInteraction
{
	public delegate void ActionSelectedHandler();
	
	public ObjectInteraction(string displayName, ActionSelectedHandler handler)
	{
		m_displayName = displayName;
		m_handler = handler;
	}
	
	public string DisplayName
	{
		get { return m_displayName; }	
	}
	
	public ActionSelectedHandler Handler
	{
		get { return m_handler; }	
	}
	
	private string m_displayName = string.Empty;
	private ActionSelectedHandler m_handler;
}
