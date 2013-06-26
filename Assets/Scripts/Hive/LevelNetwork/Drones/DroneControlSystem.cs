///////////////////////////////////////////////////////////
// 
// DroneControlSystem.cs
//
// What it does: Manages drone control
//
// Notes: This is largely just GUI-guff right now.
// 
// To-do:
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

public class DroneControlSystem : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		m_resource = 100.0f;
		Level level = FindObjectOfType(typeof(Level)) as Level;
		
		if(level != null)
		{
			m_dronesObject = level.GetLevelChild("drones", true);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(m_lerping)
		{
			m_lerpProgress += m_lerpDelta;
			float z = Camera.mainCamera.transform.position.z;
			Camera.mainCamera.transform.position = (Vector3)(Vector2.Lerp(m_lerpOrigin, m_lerpTarget, m_lerpProgress)) + new Vector3(0.0f, 0.0f, z);
			
			if(m_lerpProgress >= 1.0f)
			{
				m_lerping = false;	
			}
		}
	
	}
	
	void OnGUI()
	{
		// This uses ray-casting throughout, so just bung these here
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
		RaycastHit[] info = Physics.RaycastAll(ray, 100.0f);
		
		// Cancel selections
		if(Event.current.type == EventType.mouseDown && Input.GetMouseButtonDown(1))
		{
			if(m_selectedDrone != null)
			{
				m_selectedDrone.SetSelected(false);	
			}
			
			m_selectedDrone = null;	
		}
		
		// Left-click
		if(Event.current.type == EventType.mouseDown && Input.GetMouseButtonDown(0))
		{
			AdminDrone selectedDrone 				= null; 
			LevelNetworkSelectableNode selectedNode = null; 
			
			foreach(var currentHit in info)
			{
				selectedDrone = currentHit.collider.gameObject.GetComponent<AdminDrone>();
				selectedNode = currentHit.collider.gameObject.GetComponent<LevelNetworkSelectableNode>();
				
				if(selectedDrone != null)
				{
					if(m_selectedDrone != null)
					{
						m_selectedDrone.SetSelected(false);	
					}
					
					m_selectedDrone = selectedDrone;
					
					m_selectedDrone.SetSelected(true);
				}
				else if(selectedNode != null)
				{
					LevelNetworkSelectableNode currentNode = selectedNode;
					break;
				}
			}
			
			if(selectedDrone == null)
			{
				if(m_selectedDrone != null)
				{
					m_selectedDrone.SetSelected(false);	
				}
				
				m_selectedDrone = null;	
			}
		}
		
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
		info = Physics.RaycastAll(ray, 100.0f);
		
		bool hit = false;
		foreach(var currentHit in info)
		{
			AdminDrone drone = currentHit.collider.gameObject.GetComponent<AdminDrone>();
			if(drone != null)
			{
				HighlightDrone(drone);
				hit = true;
			}
		}
		
		if(!hit && m_highlightedDrone != null)
		{
			m_highlightedDrone.HighlightLost();
		}
		
		ShowGUISpawnList();
		ShowGUIActiveDroneList();
	}
	
	private void ShowGUISpawnList()
	{
		GUILayout.BeginArea(new Rect(10, Screen.height / 2 - 20, 130, 140));
		
		GUILayout.Label("CPU Usage: " + (100.0f - m_resource) + "%");
		
		if(m_selectedDrone != null)
		{
			GUILayout.Label("Drone Selected");
		}
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		foreach(var type in m_registeredDroneObjects)
		{
			AdminDrone drone = type.GetComponent<AdminDrone>();
			if(m_resource < drone.GetResourceUsage())
			{
				GUI.enabled = false;	
			}
			
			if(GUILayout.Button(type.name))
			{
				GameObject newDrone = GameObject.Instantiate(type) as GameObject;	
				newDrone.transform.parent = m_dronesObject.transform;
				m_activeDrones.Add(newDrone.GetComponent<AdminDrone>());
				m_resource -= drone.GetResourceUsage();
			}
			GUI.enabled = true;
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
	}
	
	private void ShowGUIActiveDroneList()
	{
		GUILayout.BeginArea(new Rect(Screen.width - 210, Screen.height / 2 - 20, 200, 240));
		
		m_droneScrollPos = GUILayout.BeginScrollView(m_droneScrollPos);
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		AdminDrone highlightedDrone = null;	
		
		foreach(var activeDrone in m_activeDrones)
		{
			Color oldColor = GUI.color;
			
			if(activeDrone == m_selectedDrone)
			{
				GUI.color = Color.blue;
			}
			else if(activeDrone == m_lastHighlightedDrone || activeDrone.Highlighted)
			{
				GUI.color = Color.green;
			}
			
			GUILayout.BeginVertical();
			
			GUILayout.Label(activeDrone.name);
			foreach(var info in activeDrone.GetInfo(false))
			{
				GUILayout.Label("-" + info);	
			}
			
			GUILayout.Box("", GUILayout.Height(1));
			
			GUILayout.EndVertical();
				
			if(Event.current.type == EventType.repaint)
			{
				if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				{
					highlightedDrone = activeDrone;
					HighlightDrone(activeDrone);
				}
			}
			
			if(Event.current.type == EventType.mouseDown && Input.GetMouseButtonDown(1) && m_lastHighlightedDrone != null)
			{
				m_lerpOrigin = Camera.mainCamera.transform.position;
				m_lerpTarget = m_lastHighlightedDrone.transform.position;
				m_lerpDelta = 1.0f / 10.0f;
				m_lerpProgress = 0.0f;
				m_lerping = true;
			}
			
			GUI.color = oldColor;
		}
		
		if(Event.current.type == EventType.repaint)
		{
			m_lastHighlightedDrone = highlightedDrone;
		}
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
	}
	
	private void HighlightDrone(AdminDrone drone)
	{
		if(m_highlightedDrone != null)
		{
			m_highlightedDrone.HighlightLost();	
		}
		
		drone.HighlightGained();
		m_highlightedDrone = drone;
	}
	
	public List<GameObject> m_registeredDroneObjects = new List<GameObject>();
	
	private List<AdminDrone> m_activeDrones 	= new List<AdminDrone>();
	private AdminDrone m_highlightedDrone 		= null;
	private AdminDrone m_selectedDrone 			= null;
	private GameObject m_dronesObject 			= null; 					// Where new Drones are to be placed in the scene-hierarchy.
	private float m_resource					= 0.0f;
	private Vector2 m_droneScrollPos			= new Vector2();
	private AdminDrone m_lastHighlightedDrone 	= null;						// This is required due to the GUI system's finickiness
	private Vector2 m_lerpTarget				= Vector2.zero;
	private Vector2 m_lerpOrigin				= Vector2.zero;
	private float m_lerpProgress				= 0.0f;
	private float m_lerpDelta					= 0.0f;
	private bool m_lerping						= false;
}
