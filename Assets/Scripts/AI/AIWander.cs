using UnityEngine;
using System.Collections;

public class AIWander : MonoBehaviour, IDetectionListener
{
	private enum State
	{
		Idle,
		Wandering,
		RouteToDetection,
		RouteToDetectionArea,
		Finished
	}
	
	public void DetectionMade(Vector3 detectionPoint)
	{
		m_lastDetectionPoint = detectionPoint;
		
		if(m_state != State.RouteToDetection)
		{
			m_state = State.RouteToDetection;	
		}
	}
	
	public void DetectionLost()
	{
			
	}
	
	public void OnTriggerEnter(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if(player != null)
		{
			m_collidedPlayer = player;		
			Debug.Log("Player found");
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if(player != null)
		{
			m_collidedPlayer = null;		
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		m_level = FindObjectOfType(typeof(Level)) as Level;
		
		if(m_level == null)
		{
			Debug.LogError("Level object not found.");	
		}
		else
		{
			m_stuck = m_level.TileBlocked((int)transform.position.x, (int)transform.position.y);
			Debug.Log("AI Stuck");
		}
	}
	
	void FixedUpdate () 
	{
#if UNITY_EDITOR
		if(m_level == null)
		{
			m_level = FindObjectOfType(typeof(Level)) as Level;
			
			if(m_level != null)
			{
				m_stuck = m_level.TileBlocked((int)transform.position.x, (int)transform.position.y);
				Debug.Log("AI Stuck");
			}
			
			// Bail for this frame regardless of whether the level was found.
			return; 
		}
#endif
		if(m_stuck)
		{
			return;	
		}
		
		if(m_collidedPlayer != null)
		{
			m_collidedPlayer.Health -= m_collidedPlayer.DamageRate;	
		}
		
		switch(m_state)
		{
			case State.Idle:
			{
				AIGraphNode currentNode = m_level.AIGraph.GetNearestNodeToPosition(transform.position);
				AIGraphNode targetNode = m_level.AIGraph.GetRandomNode();
				
				int bailout = 100;
				int iteration = 0;
				while(targetNode == currentNode || targetNode == null && iteration != bailout)
				{
					targetNode = m_level.AIGraph.GetRandomNode();
					iteration++;
				}
				
				RouteFinder routeFinder = new RouteFinder();
				m_currentRoute = routeFinder.FindRoute(m_level.AIGraph, currentNode, targetNode);
				if(m_currentRoute != null && m_currentRoute.m_routePoints.Count > 0)
				{
					m_state = State.Wandering;
				}
				break;
			}
			
		case State.Wandering:
			{
				
				Vector2 direction = m_currentRoute.m_routePoints[0].NodePosition - (Vector2)transform.position;
				
				if(direction.magnitude < 0.1f)
				{
					m_currentRoute.m_routePoints.RemoveAt(0);
				
					if(m_currentRoute.m_routePoints.Count == 0)
					{
						m_state = State.Idle;
						break;
					}
				}
			
				direction.Normalize();
			
				transform.position += (Vector3)(direction * MoveSpeed);
			
				break;
			}
			case State.RouteToDetection:
			{
				
				Vector2 direction = (Vector2)m_lastDetectionPoint - (Vector2)transform.position;
				
				if(direction.magnitude < 0.1f)
				{
					m_state = State.Idle;
					break;
				}
			
				direction.Normalize();
			
				transform.position += (Vector3)(direction * MoveSpeed);
			
				break;
			}
		}
	}
	
	void OnDrawGizmos()
	{
		if(m_currentRoute != null)
		{
			for(int i = 0; i < m_currentRoute.m_routePoints.Count; i++)
			{
				Vector2 point = m_currentRoute.m_routePoints[i].NodePosition;
				Vector2 altPoint = i > 0 ? m_currentRoute.m_routePoints[i - 1].NodePosition : m_currentRoute.m_routePoints[i].NodePosition;
				
				Gizmos.DrawLine(point, altPoint);
			}
		
		}
	}
	
	public float MoveSpeed = 0.02f;
	
	private Level m_level = null;
	private State m_state = State.Idle;
	private Route m_currentRoute = null;
	private bool m_stuck = false;
	private Vector3 m_lastDetectionPoint = Vector3.zero;
	private Player m_collidedPlayer = null;
}
