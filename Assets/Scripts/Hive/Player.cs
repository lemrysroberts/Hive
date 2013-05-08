using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	
	// Use this for initialization
	void Start () 
	{
		m_level = GameObject.FindObjectOfType(typeof(Level)) as Level;
		
		if(m_level != null)
		{
			Vector2 spawnPoint = m_level.PlayerSpawnPoint;
			
			if(spawnPoint != null)
			{
				// The spawn-point is a tile-position, so bump the player up and right to center them into a tile.
				Vector3 startPosition = transform.position;
				startPosition.x = spawnPoint.x + 0.5f;
				startPosition.y = spawnPoint.y + 0.5f;
				
				transform.position = startPosition;
			}
			else
			{
				Debug.LogWarning("No player spawn point set.");	
			}
		}
		else
		{
			Debug.LogError("No \"Level\" object found.");	
		}
	}
	
	
	Level m_level = null;
}
