using UnityEngine;
using System.Collections;

public class LevelNetworkConnection 
{
	// Pretty arbitrary
	public LevelNetworkNode startNode = null;
	public LevelNetworkNode endNode = null;
	public bool gameObjectCreated = false;
	
	// TODO: This is bollocks
	public int traversalCount = 0;
}
