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
	public override List<string> GetInteractions()
	{
		List<string> interactions = new List<string>();
		interactions.Add("Do terminal things...");
		
		return interactions;
	}

}
