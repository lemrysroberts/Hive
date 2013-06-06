using UnityEngine;
using System.Collections.Generic;

public class LevelNetworkPort : LevelNetworkCommandIssuer 
{
	public LevelNetworkPort()
	{
		m_claimable = false;	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override List<LevelNetworkCommand> GetCommandNames()
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		commands.Add(new LevelNetworkCommand("hack_port", "Hack Port"));
			
		return commands;
	}
	
	public override void IssueCommand(LevelNetworkCommand commandName)
	{
		
	}
	
}
