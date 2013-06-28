///////////////////////////////////////////////////////////
// 
// DroneStateRoute.cs
//
// What it does: Routes a drone from one location to another.
//
// Notes: Probably has a strop if you return null from either provider function. So don't.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class DroneStateRoute : IDroneState
{
	public delegate LevelNetworkNode PositionProvider();
	
	public DroneStateRoute(AdminDrone drone, PositionProvider destinationProvider, float moveTime)
	{
		m_drone					= drone;
		m_destinationProvider 	= destinationProvider;
		m_moveTime				= moveTime;
	}
	
	public void Start()
	{
		m_network 					= (GameObject.FindObjectOfType(typeof(Level)) as Level).Network;
		m_state 					= InternalState.FindingRoute;
		m_drone.m_destinationNode 	= m_destinationProvider();
		m_drone.m_targetNode 		= null;
	}
	
	public UpdateResult Update()
	{
		switch(m_state)
		{
			case InternalState.FindingRoute:
			{
				m_stateString = "Finding Route";
				m_currentRoute = m_network.RouteFinder.FindRoute(m_network.RouteGraph,	m_drone.m_originNode.AINode, m_drone.m_destinationNode.AINode);
		
				// Progress only if a route has been found and it has more than one element. Otherwise, the drone is stuck.
				if(m_currentRoute != null && m_currentRoute.m_routePoints.Count > 1)
				{
					m_state = InternalState.Routing;
					
					m_targetNode 				= null;
					m_currentRoutePointIndex 	= 0;
					m_currentLerpProgress 		= 0.0f;
					m_lerpSpeed 				= (1.0f /  m_moveTime) / ((m_currentRoute.m_routePoints[1].NodePosition - m_currentRoute.m_routePoints[0].NodePosition).magnitude);
				
					return UpdateResult.Updating;
				}
				else
				{
					return UpdateResult.Failed;
				}
			}
			
			case InternalState.Routing:
			{
				m_currentLerpProgress += m_lerpSpeed;
				m_currentLerpProgress = Mathf.Min(m_currentLerpProgress, 1.0f);
			
				float sectionPercent = 1.0f / (float)(m_currentRoute.m_routePoints.Count - 1);
				float currentProgress = (sectionPercent * m_currentRoutePointIndex) + (sectionPercent * m_currentLerpProgress);
				m_stateString = "[Routing] " + currentProgress.ToString("0.00") + "%";
				
				float z = m_drone.transform.position.z;
				m_drone.transform.position = Vector2.Lerp(m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodePosition, m_currentRoute.m_routePoints[m_currentRoutePointIndex + 1].NodePosition, m_currentLerpProgress);
				m_drone.transform.position = new Vector3(m_drone.transform.position.x, m_drone.transform.position.y, z);
				
				if(m_currentLerpProgress >= 1.0f)
				{
					m_currentRoutePointIndex++;
					m_currentLerpProgress = 0.0f;
					
					// Bail if the route is finished
					if(m_currentRoutePointIndex + 1 >= m_currentRoute.m_routePoints.Count || m_targetNode != null)
					{
						m_drone.m_originNode 		= m_drone.m_destinationNode;	
						m_currentRoutePointIndex 	= 0;
					
						return UpdateResult.Complete;
					}
					else
					{
						m_lerpSpeed = (1.0f /  m_moveTime) / ((m_currentRoute.m_routePoints[m_currentRoutePointIndex + 1].NodePosition - m_currentRoute.m_routePoints[m_currentRoutePointIndex].NodePosition).magnitude);
					}
				}
				return UpdateResult.Updating;
			}
		}
		
		return UpdateResult.Complete;
	}
	
	public string GetStateInfo()
	{
		return m_stateString;
	}
	
	public void	OnGUI() { }
	
	private PositionProvider m_destinationProvider 	= null;
	private AdminDrone m_drone						= null;
	private Route m_currentRoute 					= null;
	private float m_lerpSpeed						= 0.0f;
	private float m_moveTime						= 0.0f;
	private float m_currentLerpProgress 			= 0.0f;
	private int m_currentRoutePointIndex 			= 0;
	private LevelNetwork m_network					= null;
	private InternalState m_state					= InternalState.FindingRoute;
	private LevelNetworkNode m_targetNode			= null;
	private string m_stateString					= string.Empty;
	
	private enum InternalState
	{
		FindingRoute,
		Routing,
	}
}
