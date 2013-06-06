using UnityEngine;
using System.Collections.Generic;

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
		
		InteractiveObjectNotifier interactiveObject = other.GetComponent<InteractiveObjectNotifier>();
		if(interactiveObject != null)
		{
			m_currentNotifier = interactiveObject;
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		InteractiveObjectNotifier interactiveObject = other.GetComponent<InteractiveObjectNotifier>();
		if(interactiveObject != null && interactiveObject == m_currentNotifier)
		{
			m_currentNotifier = null;
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
	
	public void OnGUI()
	{
		if(m_currentNotifier != null)
		{
			List<string> interactions = m_currentNotifier.TargetObject.GetInteractions();
			
			GUILayout.BeginArea(new Rect(250, 300, 100, interactions.Count * 30), (GUIStyle)("Box"));
			
			foreach(var interaction in interactions)
			{
				GUILayout.Button(interaction);	
			}
			
			GUILayout.EndArea();
			
		}
	}
	
	GameObject m_grabbedObject = null;
	Level m_level = null;
	InteractiveObjectNotifier m_currentNotifier = null;
}
