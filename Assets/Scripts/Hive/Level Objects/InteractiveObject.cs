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
	public abstract List<string> GetInteractions();
	
}
