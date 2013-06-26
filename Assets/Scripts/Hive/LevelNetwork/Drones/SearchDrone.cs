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
	public SearchDrone()
	{
		DroneStateRoute routeState 			= new DroneStateRoute(this, new DroneStateRoute.PositionProvider(GetRouteDestination), 4.5f);
		DroneStateIdentify identifyState 	= new DroneStateIdentify(this, m_sessionClient, ref m_session);
		
		m_states.Add(routeState);
		m_states.Add(identifyState);
	}
	
	protected override void StartInternal() 
	{
		m_supportsSelection = false;
		
		m_level 	= FindObjectOfType(typeof(Level)) as Level;
		m_network 	= m_level.Network;
		
		LevelNetworkNode startNode 	= m_network.Nodes[0];
		transform.position 			= new Vector3(startNode.transform.position.x, startNode.transform.position.y, transform.position.z);
		m_originNode 				= startNode;
	}
			
	LevelNetworkNode GetRouteDestination()
	{
		return m_network.Nodes[Random.Range(0, m_network.Nodes.Count - 1)];	
	}
	
	public override List<string> GetInfo(bool getNodeInfo)
	{
		List<string> info = new List<string>();
		
		info.Add(m_currentStateString);
		
		if(m_originNode != null && getNodeInfo)
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
		return new List<LevelNetworkCommand>();
	}
	
	public override float GetResourceUsage()
	{
		return 10.0f;	
	}
	
	private string m_currentStateString	= string.Empty;
}