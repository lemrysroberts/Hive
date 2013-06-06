/// <summary>
/// Contains all data for a given LevelNetwork command.
/// </summary>

using UnityEngine;
using System.Collections;

public class LevelNetworkCommand
{
	public LevelNetworkCommand(string name, string displayName)
	{
		m_name 			= name;
		m_displayName 	= displayName;
	}
	
	public string Name
	{
		get { return m_name; }
	}
	
	public string DisplayName
	{
		get { return m_displayName; }
	}
	
	private string m_name 			= string.Empty;
	private string m_displayName 	= string.Empty;
}
