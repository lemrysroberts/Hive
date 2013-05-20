using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float Health = 1.0f;
	public float DamageRate = 0.01f;
	
	// Use this for initialization
	void Start () 
	{
		m_level = GameObject.FindObjectOfType(typeof(Level)) as Level;
		
		if(m_level != null)
		{
			Vector2 spawnPoint = m_level.PlayerSpawnPoint;
			
			// The spawn-point is a tile-position, so bump the player up and right to center them into a tile.
			Vector3 startPosition = transform.position;
			startPosition.x = spawnPoint.x + 0.5f;
			startPosition.y = spawnPoint.y + 0.5f;
			
			transform.position = startPosition;
		}
		else
		{
			Debug.LogError("No \"Level\" object found.");	
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<GoalItem>() != null)
		{
			m_grabbedObject = other.gameObject;
		}
	}
	
	public void OnCollisionEnter(Collision other)
	{
		//Debug.Log("Collided: " + other.collider.name);	
	}
	
	public void Update()
	{
		if(m_grabbedObject != null)
		{
			m_grabbedObject.transform.position = transform.position;	
		}
		
		if(Health <= 0.0f)
		{
			GameFlow.Instance.GameLost();
		}
	}
	
	GameObject m_grabbedObject = null;
	Level m_level = null;
}
