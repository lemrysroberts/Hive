/// <summary>
/// Route finder.
/// 
/// This is a hastily put-together A* route-finder. 
/// As with most of my code, it's not very well tested and on a different continent to optimal, so...
/// http://25.media.tumblr.com/tumblr_ma8uniqOJZ1r9n5d3o1_250.jpg
/// 
/// </summary>

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
		
		int maxIterations = 1000;
		
		// Pretty lazy, but C# defaults a bool array to false
		m_closedList = new bool[AIGraph.MaxIndex];
		m_parentList = new int[AIGraph.MaxIndex];
		
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
			//Debug.Log("No route found: Max iterations");	
		}
		else
		{
			int currentID = m_parentList[m_openHeap.GetTop().ID];
			
			while(currentID != 0)
			{
				AIGraphNode node = searchGraph.Nodes[currentID];
				
				route.m_routePoints.Add(node);
				currentID = m_parentList[currentID];
				
					
			}
			
			route = TrimRoute(route);
			
			route.m_routePoints.Reverse();
			
			//Debug.Log(" Iterated over " + iterationCount + " nodes" );	
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
		
	//	Debug.Log("Found " + toRemove.Count + " items to be removed");
		
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
			m_parentList[link.ID] = m_graph.GetNodeIndex(currentNode.NodePosition);
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
}
