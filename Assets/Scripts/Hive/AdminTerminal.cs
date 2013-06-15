using UnityEngine;
using System.Collections.Generic;

public class AdminTerminal : LevelNetworkCommandIssuer 
{
	public void Start()
	{
		m_terminal = GetComponent<Terminal>();	
		
		m_terminal.HackedStateChanged += HandleHackedStateChanged;
	}
	
	public override List<LevelNetworkCommand> GetCommands()
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		
		commands.Add(new LevelNetworkCommand("deactivate", "Deactivate"));
		
		return commands;	
	}
	
	public override void IssueCommand(LevelNetworkCommand commandName)
	{
		
	}
	
	public override List<string> GetInfoStrings()
	{
		List<string> infoStrings = new List<string>();
		return infoStrings;
	}
	
	private void HandleHackedStateChanged()
	{
		LevelNetworkNode networkNode = GetComponent<LevelNetworkNode>();
		
		if(m_terminal.m_hacked)
		{
			networkNode.SetClaimed();	
		}
	}
	
	private Terminal m_terminal = null;
}
