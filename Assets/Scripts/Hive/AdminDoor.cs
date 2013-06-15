using UnityEngine;
using System.Collections.Generic;

public class AdminDoor : LevelNetworkCommandIssuer
{
	public Material OpenMaterial;
	public Material ClosedMaterial;
		
	public Door m_door = null;
	public MeshRenderer m_renderer = null;
	

	// Use this for initialization
	void Start () 
	{
		m_door = GetComponent<Door>();
		m_renderer.material = ClosedMaterial;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_door.m_opening != m_lastOpening)
		{
			m_lastOpening = m_door.m_opening;	
			if(m_door.m_opening)
			{
				m_renderer.material = OpenMaterial;
			}
			else
			{
				m_renderer.material = ClosedMaterial;
			}
		}
	}
	
	public override List<LevelNetworkCommand> GetCommands()
	{
		List<LevelNetworkCommand> commands = new List<LevelNetworkCommand>();
		
		commands.Add(new LevelNetworkCommand("open_door", "Open Door"));
		
		return commands;
	}
	
	public override void IssueCommand(LevelNetworkCommand command)
	{
		if(command.Name == "open_door")
		{
			m_door.m_other = true;
		}
	}
	
	public override List<string> GetInfoStrings()
	{
		List<string> infoStrings = new List<string>();
		return infoStrings;
	}
			
	private bool m_lastOpening = false;
}
