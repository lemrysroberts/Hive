using UnityEngine;
using System.Collections.Generic;

public class AgentDoor : InteractiveObject
{
	public GameObject MeshObject = null;
	public Door m_door = null;
	
	private Vector3 m_originalTransform;
	private Vector3 m_openTransform;
	
	public override List<string> GetInteractions()
	{
		List<string> test = new List<string>();
		test.Add("Push button, ja?");
		
		return test;
	}

	void Start () 
	{
		m_originalTransform = transform.position;
		
		Vector3 rotated = (MeshObject.transform.rotation * Vector3.right) * transform.lossyScale.x;
		m_openTransform = (MeshObject.transform.position - rotated) ;
	}
	
	void Update () 
	{
		if(m_door.m_openProgress != m_lastProgress)
		{
			m_lastProgress = m_door.m_openProgress;
			MeshObject.transform.position = Vector3.Lerp(m_originalTransform, m_openTransform, m_door.m_openProgress);
		}
	}
	
	private float m_lastProgress = 0.0f;
}
