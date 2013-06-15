///////////////////////////////////////////////////////////
// 
// AgentTerminal.cs
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

public class AgentTerminal : InteractiveObject 
{
	public override List<ObjectInteraction> GetInteractions()
	{
		List<ObjectInteraction> interactions = new List<ObjectInteraction>();
		interactions.Add(new ObjectInteraction("Do terminal things...", HandleTerminalThing));
		
		return interactions;
	}
	
	void Start()
	{
		m_terminal = GetComponent<Terminal>();
		
		if(m_terminal == null)
		{
			Debug.LogError("GameObject does not have required \"Terminal\" script");	
		}
	}
	
	private void HandleTerminalThing()
	{
		m_terminal.m_hacked = true;
	}
	
	private Terminal m_terminal = null;

}
