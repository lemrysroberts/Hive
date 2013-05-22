using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlowWrapper : MonoBehaviour 
{
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
			level.Seed = System.DateTime.Now.Millisecond;
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
	
	public WorldView View
	{
		get { return GameFlow.Instance.View; }
		set { GameFlow.Instance.View = value; }
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
	
	[SerializeField]
	private List<GameObject> m_agentStartupObjects = new List<GameObject>();
	
	[SerializeField]
	private List<GameObject> m_adminStartupObjects = new List<GameObject>();

}
