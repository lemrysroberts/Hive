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
	
	public override List<string> GetCommandNames()
	{
		List<string> commands = new List<string>();
		
		commands.Add("Open Door");
		
		return commands;
	}
	
	public override void IssueCommand(string command)
	{
		if(command == "Open Door")
		{
			m_door.m_other = true;
		}
	}
			
	private bool m_lastOpening = false;
}
