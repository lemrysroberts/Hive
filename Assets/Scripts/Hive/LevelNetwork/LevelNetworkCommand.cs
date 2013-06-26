/// <summary>
/// Contains all data for a given LevelNetwork command.
/// </summary>

using UnityEngine;
using System.Collections;

public class LevelNetworkCommand
{
	public LevelNetworkCommand(string name, string displayName, float executionTime, float updateHeat)
	{
		m_name 			= name;
		m_displayName 	= displayName;
		m_executionTime = executionTime;
		m_updateHeat	= updateHeat;
	}
	
	public string Name
	{
		get { return m_name; }
	}
	
	public string DisplayName
	{
		get { return m_displayName; }
	}
	
	public float ExecutionTime
	{
		get { return m_executionTime; }	
	}
	
	public float UpdateHeat
	{
		get { return m_updateHeat; }	
	}
	
	private string 	m_name 			= string.Empty;
	private string 	m_displayName 	= string.Empty;
	private float 	m_executionTime	= 3.0f;
	private float 	m_updateHeat	= 0.0f;
}
