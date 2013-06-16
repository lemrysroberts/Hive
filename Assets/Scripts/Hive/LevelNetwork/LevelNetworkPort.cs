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
	
	public override List<LevelNetworkCommand> GetCommands(int permissionLevel)
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		commands.Add(new LevelNetworkCommand("hack_port", "Hack Port"));
			
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
	
	public void SetTerminalNode(LevelNetworkNode terminalNode)
	{
		m_connectedTerminalNode = terminalNode;
	}
	
	private void TerminalNodeClaimed()
	{
		LevelNetworkNode networkNode = GetComponent<LevelNetworkNode>();
	}
	
	private LevelNetworkNode m_connectedTerminalNode = null;
	
	private bool m_hackInProgress = false;
	
}
