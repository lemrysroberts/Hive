//////////////////////////////////////////////////////////////
// 
// RouteFinder.cs
//
// What it does: Hastily put-together A* route-finder. 
//  			 As with most of my code, it's not very well tested and on a different continent to optimal, so...
//  			 http://25.media.tumblr.com/tumblr_ma8uniqOJZ1r9n5d3o1_250.jpg
// 
// Notes: http://www.youtube.com/watch?feature=player_detailpage&v=F_XaIuw6K6Q#t=8s
//
// To-do: 	- Make this a request-based system and set a hard limit on the number of routes calculated per-rame.
//			- Remove a lot of the data-heavy resetting. 
//			- Maybe a better heuristic, though it seems to work for now.
//
///////////////////////////////////////////////////////////

using UnityEngine; 					// This is only needed for that sexy maths.
using System.Collections.Generic;

public class RouteFinder
{
	public Route FindRoute(AIGraph searchGraph, AIGraphNode start, AIGraphNode end)
	{
		m_graph = searchGraph;
		Route route = new Route();
		m_targetPos = end.NodePosition;
		
		m_openHeap.Reset();
		
		int maxIterations = 10000;
		
		// Pretty lazy, but C# defaults a bool array to false
		m_closedList = new bool[searchGraph.Nodes.Count];
		m_parentList = new int[searchGraph.Nodes.Count]; // No such excuse for ints...
		
		m_openHeap.Insert(start, (end.NodePosition - start.NodePosition).magnitude);
		
		int iterationCount = 0;
		while(m_openHeap.HasItems() && m_openHeap.GetTop().NodePosition != end.NodePosition && iterationCount < maxIterations)
		{
			UpdateHeap();
			iterationCount++;
		}
		
		if(!m_openHeap.HasItems())
		{
			Debug.Log("No route found");
			
			return route;
		}
		
		if(iterationCount == maxIterations)
		{
			Debug.LogWarning("No route found: Max iterations");	
		}
		else
		{
			// Unwind the route.
			int currentID = m_parentList[m_openHeap.GetTop().ID];
			
			route.m_routePoints.Add(end);
			
			while(currentID != 0)
			{
				AIGraphNode node = searchGraph.Nodes[currentID];
				
				route.m_routePoints.Add(node);
				currentID = m_parentList[currentID];
			}
			
			route = TrimRoute(route);
			
			route.m_routePoints.Reverse();
		}
		
		return route;
	}
	
	private Route TrimRoute(Route route)
	{
		List<AIGraphNode> toRemove = new List<AIGraphNode>();
		
		for(int item = 0; item < route.m_routePoints.Count; item++)
		{
			bool shortcutFound = false;
			for(int other = route.m_routePoints.Count - 1; other > item + 1 && !shortcutFound; other--)
			{
				if(route.m_routePoints[item].NodeLinks.Contains(route.m_routePoints[other]))
				{
					for(int toRemoveID = item + 1; toRemoveID < other; toRemoveID++)
					{
						toRemove.Add(route.m_routePoints[toRemoveID]);	
					}
					
					item = other;
					shortcutFound = true;
				}
			}
		}
		
		foreach(var deadNode in toRemove)
		{
			route.m_routePoints.Remove(deadNode);	
		}
		
		return route;
	}
	
	private void UpdateHeap()
	{
		// Remove the current node from the open-list
		AIGraphNode currentNode = m_openHeap.RemoveTop();
		
		// Add it to the closed-list
		m_closedList[currentNode.ID] = true;
		
		// Add all open links
		foreach(var link in currentNode.NodeLinks)
		{
			if(m_closedList[link.ID])
			{
				continue;	
			}
			
			// So, a better metric than this, yeah?
			m_openHeap.Insert(link, (m_targetPos - link.NodePosition).magnitude);
			int index = currentNode.ID;
			m_parentList[link.ID] = index; 
		}
	}
	
	private AIGraph m_graph;
	private Vector2 m_targetPos;
	private int[] m_parentList;
	private bool[] m_closedList;
	private BinaryHeap<AIGraphNode> m_openHeap = new BinaryHeap<AIGraphNode>();
}

// TODO: This should eventually hold information about what happened when trying to grab a route.
//		 FailedBlocked, FailedIterations, etc.
public class Route
{
	public List<AIGraphNode> m_routePoints = new List<AIGraphNode>();
	
#if UNITY_EDITOR
	public void DrawGizmos()
	{
		Vector3 boxSize = new Vector3(0.4f, 0.4f, 0.2f);	
		Gizmos.color = Color.red;
		for(int i = 0; i < m_routePoints.Count; i++)
		{
			Vector3 point = m_routePoints[i].NodePosition;
			Vector3 altPoint = i > 0 ? m_routePoints[i - 1].NodePosition : m_routePoints[i].NodePosition;
			
			point.z = -3.0f;
			altPoint.z = -3.0f;
			
			Gizmos.DrawCube(point, boxSize);
			Gizmos.DrawLine(point, altPoint);
		}
		
		Gizmos.color = Color.green;
		boxSize = new Vector3(0.4f, 0.4f, 0.3f);
	}
#endif
}
