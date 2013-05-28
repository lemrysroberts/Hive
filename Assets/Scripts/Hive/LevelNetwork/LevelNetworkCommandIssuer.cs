/// <summary>
/// 
/// This thing just defines some abstract methods for getting function names
/// and responding to command requests.
/// 
/// Yes, this should be an interface, but I can't be fucked to write a load of custom-editor
/// classes just to compensate for Unity's interface blindness.
/// Also, horribly named.
/// 
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public abstract class LevelNetworkCommandIssuer : MonoBehaviour 
{
	public abstract List<string> GetCommandNames();
	public abstract void IssueCommand(string commandName);
}
