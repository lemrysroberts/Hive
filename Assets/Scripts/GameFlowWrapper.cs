using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlowWrapper : MonoBehaviour 
{
	public GameObject LevelObject;
		
	// Use this for initialization
	void Start () 
	{
		GameFlow.Instance.AgentStartupItems = AgentStartupItems;
		GameFlow.Instance.AdminStartupItems = AdminStartupItems;
		
		GameFlow.Instance.LevelObject = LevelObject;
		GameFlow.Instance.Begin();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameFlow.Instance.Update();
	}
	
	void OnLevelWasLoaded(int levelID)
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
		
		if(GameFlow.Instance.View == WorldView.Admin)
		{
			TileManager.TileSetFilename = "tilesetalt";
			
			
			if(player != null)
			{
				player.GetComponent<KinematicKeyMove>().enabled = false;	
			}
			
			if(camera != null)
			{
				camera.GetComponent<FollowPlayer>().enabled = false;	
				camera.GetComponent<DEBUG_KeyMove>().enabled = true;
			}
			
		}
		else
		{
			TileManager.TileSetFilename = "tileset";	
			
			if(camera != null)
			{
				camera.GetComponent<FollowPlayer>().enabled = true;	
				camera.GetComponent<DEBUG_KeyMove>().enabled = false;
			}
		}
		
		Level level = FindObjectOfType(typeof(Level)) as Level;
		//level.Load(GameFlow.Instance.CurrentLevel);
		LevelGen generator = new LevelGen(level);
		generator.GenerateLevel(System.DateTime.Now.Millisecond, false);
		Debug.Log("Loading: " + GameFlow.Instance.CurrentLevel);
	}
	
	private void OnPlayerConnected()
	{
		Debug.Log("Client Connected");	
		if(GameFlow.Instance.View == WorldView.Agent)
		{
			Level level = FindObjectOfType(typeof(Level)) as Level;
			level.SerialiseToNetwork(GameFlow.Instance.CurrentLevel);
			level.SyncNPCs();
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
	
	[SerializeField]
	private List<GameObject> m_agentStartupObjects = new List<GameObject>();
	
	[SerializeField]
	private List<GameObject> m_adminStartupObjects = new List<GameObject>();

}
