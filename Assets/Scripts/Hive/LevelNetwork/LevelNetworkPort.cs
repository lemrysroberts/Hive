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
	
	public override List<LevelNetworkCommand> GetCommands()
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		commands.Add(new LevelNetworkCommand("hack_port", "Hack Port"));
			
		return commands;
	}
	
	public override void IssueCommand(LevelNetworkCommand command)
	{
		if(command.Name == "hack_port")
		{
			m_connectedTerminalNode.SetClaimed();
			LevelNetworkNode myNode = GetComponent<LevelNetworkNode>();
			m_connectedTerminalNode.AddLink(myNode);
			
		}
	}
	
	public override List<string> GetInfoStrings()
	{
		List<string> infoStrings = new List<string>();
		return infoStrings;
	}
	
	public void SetTerminalNode(LevelNetworkNode terminalNode)
	{
		m_connectedTerminalNode = terminalNode;
		
		m_connectedTerminalNode.NodeClaimed += TerminalNodeClaimed;
	}
	
	private void TerminalNodeClaimed()
	{
		LevelNetworkNode networkNode = GetComponent<LevelNetworkNode>();
		
			networkNode.SetClaimed();	
	}
	
	private LevelNetworkNode m_connectedTerminalNode = null;
	
	private bool m_hackInProgress = false;
	
}
