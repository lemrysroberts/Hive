using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlowWrapper : MonoBehaviour 
{
	void OnEnable()
	{
		if(GameFlow.Instance.View == WorldView.None)
		{
			GameFlow.Instance.View = m_view;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		GameFlow.Instance.AgentStartupItems = AgentStartupItems;
		GameFlow.Instance.AdminStartupItems = AdminStartupItems;
		
		GameFlow.Instance.Begin();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameFlow.Instance.Update();
	}
	
	void OnLevelWasLoaded(int levelID)
	{
		if(GameFlow.Instance.View == WorldView.Admin)
		{
			TileManager.TileSetFilename = "tilesetalt";	
		}
		
		Level level = FindObjectOfType(typeof(Level)) as Level;
		if(level != null)
		{
			int customSeed = GameFlow.Instance.CustomSeed;
			level.Seed = customSeed == -1 ? System.DateTime.Now.Millisecond : customSeed;
			LevelGenerator generator = new LevelGenerator(level);
			generator.GenerateLevel(level.Seed, false);
		}
	}
	
	private void OnPlayerConnected()
	{
		Debug.Log("Client Connected");	
		if(GameFlow.Instance.View == WorldView.Agent)
		{
			Level level = FindObjectOfType(typeof(Level)) as Level;
			level.SerialiseToNetwork(GameFlow.Instance.CurrentLevel);
			level.SerialiseLevelObjectIDs();
		}
	}
	
	public void OnGUI()
	{
		GUI.Label(new Rect((Screen.width / 2.0f) - 20.0f, 10.0f, 40.0f, 30.0f), Application.loadedLevelName);	
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
	
	public WorldView View
	{
		get { return m_view; }
		set 
		{ 
			m_view = value; 
			GameFlow.Instance.View = value;
		}
	}
	
	[SerializeField]
	private List<GameObject> m_agentStartupObjects = new List<GameObject>();
	
	[SerializeField]
	private List<GameObject> m_adminStartupObjects = new List<GameObject>();
	
	[SerializeField]
	private WorldView m_view = WorldView.Agent;

}
