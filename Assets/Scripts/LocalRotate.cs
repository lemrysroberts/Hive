///////////////////////////////////////////////////////////
// 
// LocalRotate.cs
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

public class LocalRotate : MonoBehaviour 
{
	public float Speed = 0.01f;
	
	// Update is called once per frame
	void Update () 
	{
		m_rotation += Speed;
		
		if(m_rotation > 360.0f)
		{
			m_rotation -= 360.0f;	
		}
		
		transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_rotation));
	}
	
	private float m_rotation = 0.0f;
}