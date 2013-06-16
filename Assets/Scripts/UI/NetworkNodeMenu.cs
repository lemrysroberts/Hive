///////////////////////////////////////////////////////////
// 
// NetworkNodeMenu.cs
//
// What it does: GUI for interacting with drones.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class NetworkNodeMenu : MonoBehaviour 
{
	void Update () 
	{
    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
		RaycastHit[] info = Physics.RaycastAll(ray, 100.0f);
		
		bool hit = false;
		foreach(var currentHit in info)
		{
			if(currentHit.collider.gameObject.GetComponent<AdminDrone>() != null)
			{
				hit = true;
				m_hoverObject = currentHit.collider.gameObject;
				
				if(Input.GetMouseButtonUp(0))
				{
					m_hitObject = currentHit.collider.gameObject;	
				}
				
				break;
			}
		}
		
		if(!hit)
		{
			m_hoverObject = null;	
		}
		
		if(Input.GetMouseButtonUp(1))
		{
			m_hitObject = null;	
		}
	}
	
	void OnGUI()
	{
		AdminDrone drone = null;
		
		if(m_hitObject != null)
		{
			drone = m_hitObject.GetComponent<AdminDrone>();
		}
		else if( m_hoverObject != null)
		{
			drone = m_hoverObject.GetComponent<AdminDrone>();	
		}
		
		if(drone != null)
		{
			if(drone.GetCommands().Count > 0 || drone.GetInfo().Count > 0)
			{
				Vector3 altVec = drone.transform.position;
				var point = Camera.mainCamera.WorldToScreenPoint(altVec);
				point = GUIUtility.ScreenToGUIPoint(point);
				
				GUILayout.BeginArea(new Rect(point.x + m_objectOffset.x, (Screen.height - point.y) + m_objectOffset.y, 200.0f, 200.0f), (GUIStyle)("Box"));
				GUILayout.Box(drone.name);
				
				
				
				GUILayout.BeginVertical((GUIStyle)("Box"));
				m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
				
				GUILayout.Label("Commands");
				foreach(var current in drone.GetCommands())
				{
					if(GUILayout.Button(current.DisplayName))
					{
						//node.IssueCommand(current);
						m_hitObject = null;
						break;
					}
				}
				
				GUILayout.EndVertical();
				GUILayout.EndScrollView();
				
				GUILayout.Box("", GUILayout.Height(1));
				
				m_infoScrollPos = GUILayout.BeginScrollView(m_infoScrollPos);
				
				GUILayout.BeginVertical((GUIStyle)("Box"));
				
				GUILayout.Label("Node Info");
				foreach(var current in drone.GetInfo())
				{
					GUILayout.Label(current);	
				}
				
				
				
				GUILayout.EndVertical();
				GUILayout.EndScrollView();
				
				GUILayout.EndArea();
			}
			else
			{
				m_hitObject = null;	
			}
		}
	}
	
	private Vector2 m_objectOffset = new Vector2(20.0f, 20.0f);
	private GameObject m_hitObject = null;	
	private GameObject m_hoverObject = null;
	private Vector2 m_scrollPos;
	private Vector2 m_infoScrollPos;
}

