using UnityEngine;
using System.Collections.Generic;

public class AdminTerminal : LevelNetworkCommandIssuer 
{
	public override List<LevelNetworkCommand> GetCommandNames()
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		
		commands.Add(new LevelNetworkCommand("deactivate", "Deactivate"));
		
		return commands;	
	}
	
	public override void IssueCommand(LevelNetworkCommand commandName)
	{
		
	}
	
}
