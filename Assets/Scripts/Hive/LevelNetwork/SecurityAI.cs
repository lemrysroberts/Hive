using UnityEngine;
using System.Collections.Generic;

public class SecurityAI : MonoBehaviour 
{
	public float MoveTime = 5.0f;
	
	private LevelNetwork m_network 	= null;
	private Level m_level 			= null;
	
	// Use this for initialization
	void Start () 
	{
		Reset ();
	}
	
	void Reset()
	{
		m_level = FindObjectOfType(typeof(Level)) as Level;
		m_network = m_level.Network;
		
		int startIndex = Random.Range(0, m_network.Nodes.Count);
		LevelNetworkNode startNode = m_network.Nodes[startIndex];
		transform.position = new Vector3(startNode.transform.position.x, startNode.transform.position.y, transform.position.z);
		
		m_originNode = startNode;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		switch(m_state)
		{
			case AIState.searching:
			{
				int lowestIndex = int.MaxValue;
				foreach(var currentConnection in m_originNode.Connections)
				{
					if(currentConnection.traversalCount < lowestIndex)
					{
						lowestIndex = currentConnection.traversalCount;	
					}
				}
			
				List<LevelNetworkConnection> validConnections = new List<LevelNetworkConnection>();
				foreach(var currentConnection in m_originNode.Connections)
				{
					if(currentConnection.traversalCount == lowestIndex)
					{
						validConnections.Add(currentConnection);
					}
				}
			
				int randomIndex = Random.Range(0, validConnections.Count - 1);
				LevelNetworkConnection connection = validConnections[randomIndex];
				connection.traversalCount++;
			
				m_destinationNode = connection.startNode == m_originNode ? connection.endNode : connection.startNode;
			
				m_progress = 0.0f;
			
				m_state = AIState.routing;
				m_lerpSpeed = (1.0f /  MoveTime) / ((m_originNode.transform.position - m_destinationNode.transform.position).magnitude);
			
			
				break;
			}
			
			case AIState.routing:
			{
				// TODO: This can be a valid case when the level is reset. Sort loading properly.
				if(m_originNode == null || m_destinationNode == null)
				{
					int startIndex = Random.Range(0, m_network.Nodes.Count);
					LevelNetworkNode startNode = m_network.Nodes[startIndex];
					transform.position = new Vector3(startNode.transform.position.x, startNode.transform.position.y, transform.position.z);
					
					m_originNode = startNode;
				
					m_state = AIState.searching;
				
					return;
				}
			
				m_progress += m_lerpSpeed;
				if(m_progress < 0.99f)
				{
					Vector3 newPosition = Vector3.Lerp(m_originNode.transform.position, m_destinationNode.transform.position, m_progress);
					transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z); // Preserve the editor-set depth.
				}
				else
				{
					m_originNode = m_destinationNode;
					m_state = AIState.searching;
				}
			
				break;
			}
		}
	}
	
	private LevelNetworkNode m_originNode = null;
	private LevelNetworkNode m_destinationNode = null;
	private AIState m_state = AIState.searching;
	private float m_progress = 0.0f;
	private float m_lerpSpeed = 0.0f;
	
	
	private enum AIState
	{
		routing,
		searching
	}
}
