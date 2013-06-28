///////////////////////////////////////////////////////////
// 
// FollowCameraOffsetZ.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

public class FollowCameraOffsetZ : MonoBehaviour 
{
	public Camera m_camera = null;
	private float m_offsetZ = 0.0f;
	
	// Use this for initialization
	void Start () 
	{
		m_offsetZ = transform.position.z;
		
		float size = m_camera.orthographicSize * 2.0f;
		transform.localScale = new Vector3(size * m_camera.aspect, size, 1.0f);
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		transform.position = new Vector3(m_camera.transform.position.x, m_camera.transform.position.y, m_camera.transform.position.z + 1.0f);
		//transform.localScale = 
	}
}