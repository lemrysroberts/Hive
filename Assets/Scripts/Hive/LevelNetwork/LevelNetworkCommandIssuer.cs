/// <summary>
/// 
/// This thing just defines some abstract methods for getting function names
/// and responding to command requests.
/// 
/// Yes, this should be an interface, but I can't be fucked to write a load of custom-editor
/// classes just to compensate for Unity's interface blindness.
/// Also, horribly named.
/// 
/// Update: I've found some clunky data to store in here. In your face, cleanliness!
/// 
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public abstract class LevelNetworkCommandIssuer : MonoBehaviour 
{
	public abstract List<LevelNetworkCommand> GetCommands(int permissionLevel);
	public abstract void IssueCommand(LevelNetworkCommand commandName);
	public abstract List<string> GetInfoStrings();
	
	public bool Claimable
	{
		get { return m_claimable; }
	}
	
	protected bool m_claimable = true;
}
