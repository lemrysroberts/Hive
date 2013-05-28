using UnityEngine;
using System.Collections.Generic;

public class AdminTerminal : LevelNetworkCommandIssuer 
{
	public override List<string> GetCommandNames()
	{
		List<string> commands = new List<string>();
		
		commands.Add("Deactivate");
		
		return commands;	
	}
	
	public override void IssueCommand(string commandName)
	{
		
	}
	
}
