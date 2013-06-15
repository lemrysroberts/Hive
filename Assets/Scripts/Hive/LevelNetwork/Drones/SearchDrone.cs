///////////////////////////////////////////////////////////
// 
// SearchDrone.cs
//
// What it does: Moves from node-to-node using a simple AI state-machine.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class SearchDrone : AdminDrone 
{

	// Use this for initialization
	void Start () 
	{
		m_level = FindObjectOfType(typeof(Level)) as Level;
		m_network = m_level.Network;
		
		int startIndex = Random.Range(0, m_network.Nodes.Count);
		LevelNetworkNode startNode = m_network.Nodes[startIndex];
		transform.position = new Vector3(startNode.transform.position.x, startNode.transform.position.y, transform.position.z);
		
		m_originNode = startNode;
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		switch(m_state)
		{
		case State.Idle:
		{
			if(m_targetNode != null)
			{
				m_currentRoute = m_network.RouteFinder.FindRoute(m_network.RouteGraph, m_originNode.AINode, m_targetNode.AINode);
				
				if(m_currentRoute != null && m_currentRoute.m_routePoints.Count > 1)
				{
					m_destinationNode = m_targetNode;
					m_state = State.Routing;
					
					m_targetNode 				= null;
					m_currentRoutePointIndex 	= 0;
					m_currentLerpProgress 		= 0.0f;
					m_lerpSpeed 				= (1.0f /  MoveTime) / ((m_currentRoute.m_routePoints[1].NodePosition - m_currentRoute.m_routePoints[0].NodePosition).magnitude);
					
					Debug.Log("New Target...");
				}
			}
			
			
			break;
		}
			
		case State.Routing:
		{
			m_currentLerpProgress += m_lerpSpeed;
			m_currentLerpProgress = Mathf.Min(m_currentLerpProgress, 1.0f);
			
			transform.position = Vector2.Lerp(m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodePosition, m_currentRoute.m_routePoints[m_currentRoutePointIndex + 1].NodePosition, m_currentLerpProgress);
			
			if(m_currentLerpProgress >= 1.0f)
			{
				m_currentRoutePointIndex++;
				m_currentLerpProgress = 0.0f;
				
				// Bail if the route is finished or a new route has been requested.
				if(m_currentRoutePointIndex + 1 >= m_currentRoute.m_routePoints.Count || m_targetNode != null)
				{
					if(m_targetNode != null)
					{
						m_originNode = m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodeObject as LevelNetworkNode;		
					}
					else
					{
						m_originNode = m_destinationNode;	
					}
					
					m_currentRoutePointIndex = 0;
					m_state = State.Idle;
				}
				else
				{
					m_lerpSpeed = (1.0f /  MoveTime) / ((m_currentRoute.m_routePoints[m_currentRoutePointIndex + 1].NodePosition - m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodePosition).magnitude);
				}
			}
			
			break;
		}
			
		}
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if(m_currentRoute != null)
		{
			m_currentRoute.DrawGizmos();	
		}
	}
#endif
	
	void OnGUI()
	{
		if(Event.current.type == EventType.mouseDown && Input.GetMouseButtonDown(0) && m_selected)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
			RaycastHit[] info = Physics.RaycastAll(ray, 100.0f);
			
			foreach(var currentHit in info)
			{
				if(currentHit.collider.gameObject.GetComponent<LevelNetworkSelectableNode>() != null)
				{
					LevelNetworkSelectableNode currentNode =  currentHit.collider.gameObject.GetComponent<LevelNetworkSelectableNode>();
					m_targetNode = currentNode.m_node;
					
					break;
				}
			}
		}
		
		if(GUI.Button(new Rect(10, Screen.height / 2 - 20, 130, 40), m_selected ? "Drone Selected" : "Select Drone"))
		{
			if(m_selectedDrone != null)
			{
				m_selectedDrone.m_selected = false;
			}
			
			m_selected = true;
			m_selectedDrone = this;
		}
	}
	
	private enum State
	{
		Idle,
		Routing
	}
	
	public static SearchDrone m_selectedDrone 	= null;
	public float MoveTime 						= 8.0f;
	
	private LevelNetworkNode m_targetNode		= null;
	private State m_state 						= State.Idle;
	private Route m_currentRoute 				= null;
	private float m_currentLerpProgress 		= 0.0f;
	private float m_currentLerpSpeed 			= 0.0f;
	private int m_currentRoutePointIndex 		= 0;
	private bool m_selected						= false;
}