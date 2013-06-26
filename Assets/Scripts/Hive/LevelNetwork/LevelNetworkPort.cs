using UnityEngine;
using System.Collections.Generic;

public class LevelNetworkPort : LevelNetworkCommandIssuer 
{
	public override List<LevelNetworkCommand> GetCommands(int permissionLevel)
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		commands.Add(new LevelNetworkCommand("hack_port", "Hack Port", 10.0f, 1.0f));
			
		return commands;
	}
	
	public override void IssueCommand(LevelNetworkCommand command)
	{
	}
	
	public override List<string> GetInfoStrings()
	{
		List<string> infoStrings = new List<string>();
		return infoStrings;
	}
}
