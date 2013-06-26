///////////////////////////////////////////////////////////
// 
// HackDrone.cs
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

public class HackDrone : AdminDrone 
{
	public bool DrawRoute = false;
	
	public override event System.Action Activated;
	public override event System.Action Deactivated;
	
	// Use this for initialization
	void Start () 
	{
		m_level = FindObjectOfType(typeof(Level)) as Level;
		m_network = m_level.Network;
		
		int startIndex = 0;//Random.Range(0, m_network.Nodes.Count);
		LevelNetworkNode startNode = m_network.Nodes[startIndex];
		transform.position = new Vector3(startNode.transform.position.x, startNode.transform.position.y, transform.position.z);
		
		m_originNode = startNode;
		
		m_sessionClient = new NodeSessionClient();
		m_sessionClient.ClientName = "User1134";
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		switch(m_state)
		{
		case State.Idle:
		{
			LevelNetworkNode currentNearest = null;
			float currentDistance = float.MaxValue;
			
			// Find the nearest unclaimed node
			List<LevelNetworkNode> validNodes = m_network.UnidentifiedNodes;
			foreach(var node in validNodes)
			{
				if(node.Claimant == null)
				{
					float distance = (node.transform.position - m_originNode.transform.position).sqrMagnitude;
					if(distance  < currentDistance)
					{
						currentNearest = node;
						currentDistance = distance;
					}
				}
			}
			
			if(currentNearest != null)
			{
				m_targetNode = currentNearest;
				m_currentRoute = m_network.RouteFinder.FindRoute(m_network.RouteGraph, m_originNode.AINode, m_targetNode.AINode);
				m_targetNode.Claimant = this;
				
				if(m_currentRoute != null && m_currentRoute.m_routePoints.Count > 1)
				{
					m_destinationNode = m_targetNode;
					m_state = State.Routing;
					
					m_targetNode 				= null;
					m_currentRoutePointIndex 	= 0;
					m_currentLerpProgress 		= 0.0f;
					m_lerpSpeed 				= (1.0f /  MoveTime) / ((m_currentRoute.m_routePoints[1].NodePosition - m_currentRoute.m_routePoints[0].NodePosition).magnitude);
				}
			}
			
			break;
		}
			
		case State.Routing:
		{
			m_currentLerpProgress += m_lerpSpeed;
			m_currentLerpProgress = Mathf.Min(m_currentLerpProgress, 1.0f);
			
			float z = transform.position.z;
			transform.position = Vector2.Lerp(m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodePosition, m_currentRoute.m_routePoints[m_currentRoutePointIndex + 1].NodePosition, m_currentLerpProgress);
			transform.position = new Vector3(transform.position.x, transform.position.y, z);
			
			if(m_currentLerpProgress >= 1.0f)
			{
				m_currentRoutePointIndex++;
				m_currentLerpProgress = 0.0f;
				
				LevelNetworkNode node = m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodeObject as LevelNetworkNode;
					node.Heat += 0.1f;
				
				// Bail if the route is finished or a new route has been requested.
				if(m_currentRoutePointIndex + 1 >= m_currentRoute.m_routePoints.Count || m_targetNode != null)
				{
					if(m_targetNode != null)
					{
						m_originNode = m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodeObject as LevelNetworkNode;		
						m_state = State.Idle;
					}
					else
					{
						m_originNode = m_destinationNode;	
						m_state = State.Identifying;
					}
					
					m_currentRoutePointIndex = 0;
					
				}
				else
				{
					m_lerpSpeed = (1.0f /  MoveTime) / ((m_currentRoute.m_routePoints[m_currentRoutePointIndex + 1].NodePosition - m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodePosition).magnitude);
				}
			}
			
			break;
		}
			
		case State.Identifying:
		{
			if(m_targetNode != null)
			{
				if(m_session != null)
				{
					m_originNode.EndSession(m_session);
				}
				m_state = State.Idle;
				return;
			}	
			
			m_session = m_originNode.CreateSession(m_sessionClient);
			
			List<LevelNetworkCommand> commands =  m_originNode.GetCommands(m_session);
			
			foreach(var command in commands)
			{
				if(command.Name == "identify")
				{
					m_request = m_session.IssueCommand(command);
					m_state = State.Identified;
					if(Activated != null)
					{
						Activated();	
					}
					
					return;
				}
			}
			
			m_state = State.Idle;
			
			break;
		}
			
		case State.Identified:
		{
			if(m_targetNode != null || m_originNode == null)
			{
				if(m_session != null)
				{
					m_originNode.EndSession(m_session);
				}
				m_state = State.Idle;
				return;
			}
			
			if(m_request.State == LevelNetworkCommandRequest.CommandState.Completed)
			{
				m_originNode.Claimant = null;
				m_originNode.SetIdentified();
				m_state = State.BeginHack;
				
				if(Deactivated != null)
				{
					Deactivated();	
				}
			}
			
			break;	
		}
			
		case State.BeginHack:
		{
			
			List<LevelNetworkCommand> commands =  m_originNode.GetCommands(m_session);
			
			foreach(var command in commands)
			{
				if(command.Name == "hack_clearance")
				{
					m_request = m_session.IssueCommand(command);
					m_state = State.Hacking;
					if(Activated != null)
					{
						Activated();	
					}
					
					return;
				}
			}
			
			m_state = State.Idle;
			
			break;
		}
			
		case State.Hacking:
		{
			m_currentStateString = "Testing Key: " + Random.Range(1, 8000);
			if(m_targetNode != null || m_originNode == null)
			{
				if(m_session != null)
				{
					m_originNode.EndSession(m_session);
				}
				m_state = State.Idle;
				return;
			}
			
			if(m_request.State == LevelNetworkCommandRequest.CommandState.Completed)
			{
				m_state = State.Idle;
				
				if(Deactivated != null)
				{
					Deactivated();	
				}
			}
			
			break;	
		}
			
		}
	}
	
	public override List<string> GetInfo(bool getNodeInfo)
	{
		List<string> info = new List<string>();
		
		info.Add(m_currentStateString);
		
		if(getNodeInfo)
		{
			foreach(var logEntry in m_originNode.ActivityLog)
			{
				info.Add(logEntry);	
			}
		}
		
		return info;
	}
	
	public override List<LevelNetworkCommand> GetCommands()
	{
		if(m_state == State.Identified)
		{
			return m_originNode.GetCommands(m_session);	
		}
		
		return new List<LevelNetworkCommand>();
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if(m_currentRoute != null && DrawRoute)
		{
			m_currentRoute.DrawGizmos();	
		}
	}
#endif
	
	private enum State
	{
		Idle,
		Routing,
		Identifying,
		Identified,
		BeginHack,
		Hacking
	}
	
	public static SearchDrone m_selectedDrone 	= null;
	private float MoveTime 						= 2.0f;
	
	private LevelNetworkNode m_targetNode		= null;
	private State m_state 						= State.Idle;
	private Route m_currentRoute 				= null;
	private float m_currentLerpProgress 		= 0.0f;
	private int m_currentRoutePointIndex 		= 0;
	private bool m_selected						= false;
	private NodeSessionClient m_sessionClient	= null;
	private NodeSession m_session				= null;
	private LevelNetworkCommandRequest m_request = null;
	private string m_currentStateString			= string.Empty;
}
