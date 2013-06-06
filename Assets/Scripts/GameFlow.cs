using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Because I. Love. FLOW.
/// </summary>

public enum WorldView
{
	Agent, 
	Admin,
	
	None
}

[Serializable]
public class GameFlow
{
	[SerializeField]
	public WorldView View = WorldView.None;
	
	public string CurrentLevel
	{
		get; set;	
	}
	
	private static GameFlow m_instance = null;	
	
	public static GameFlow Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new GameFlow();
				
			}
			
			return m_instance;
		}
	}
	
	public void Begin()
	{
		if(View == WorldView.Agent)
		{
			BeginAgent();	
		}
		else if(View ==	WorldView.Admin)
		{
			BeginAdmin();	
		}
	}
	
	public void Update()
	{
		
	}
	
	public void GameWon()
	{
		Application.LoadLevel("Menu");
	}
	
	public void GameLost()
	{
		Application.LoadLevel("Menu");
	}
	
	private void BeginAgent()
	{
	}
	
	private void BeginAdmin()
	{
		foreach(GameObject startObject in m_adminStartupObjects)
		{
			GameObject.Instantiate(startObject);	
		}
	}
	
	private GameFlow()
	{
		CustomSeed = -1;
	}
	
	public List<GameObject> AgentStartupItems
	{
		get { return m_agentStartupObjects; }
		set { m_agentStartupObjects = value; }
	}
	
	public List<GameObject> AdminStartupItems
	{
		get { return m_adminStartupObjects; }
		set { m_adminStartupObjects = value; }
	}
	
	public int CustomSeed { get; set; }
	
	private List<GameObject> m_agentStartupObjects 	= new List<GameObject>();
	private List<GameObject> m_adminStartupObjects 	= new List<GameObject>();
}
