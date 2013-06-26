///////////////////////////////////////////////////////////
// 
// DroneAnimate.cs
//
// What it does: Does some silly rotation to make drones look busier.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AdminDrone))]
public class DroneRenderer : MonoBehaviour 
{
	private  float RotationSpeed = 3.0f;
	private GameObject m_selectionRing = null;
	
	// Use this for initialization
	void Start () 
	{
		m_rotationSpeed = 0.0f;
		m_drone = GetComponent<AdminDrone>();
		m_drone.Activated += new System.Action(HandleActivated);
		m_drone.Deactivated += new System.Action(HandleDeactivated);
		m_drone.Selected += new System.Action(HandleSelected);
		m_drone.Deselected += new System.Action(HandleDeselected);
		m_drone.HighLightStateChanged += new AdminDrone.StateChangedHandler(HandleHighlightedStateChanged);
		
		m_defaultColor = renderer.sharedMaterial.GetColor("_Color");
		
		for(int childIndex = 0; childIndex < transform.GetChildCount(); childIndex++)
		{
			GameObject currentChild = transform.GetChild(childIndex).gameObject;
			if(currentChild.GetComponent<MeshRenderer>() != null)
			{
				m_selectionRing = currentChild;
				m_selectionRing.SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_rotation += (m_rotationMultiplier * m_rotationSpeed);
		if(m_rotation > 360.0f)
		{
			m_rotation -= 360.0f;	
		}
		
		transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_rotation));
	}
	
	private void HandleActivated()
	{
		m_rotationSpeed = RotationSpeed;
	}
	
	private void HandleDeactivated()
	{
		m_rotationSpeed = 0.0f;
	}
	
	private void HandleSelected()
	{
		renderer.material.SetColor("_Color", Color.green);
	}
	
	private void HandleDeselected()
	{
		renderer.material.SetColor("_Color", m_defaultColor);
	}
	
	private void HandleHighlightedStateChanged(bool highlightState)
	{
		if(highlightState)
		{
			if(m_selectionRing != null)
			{
				//Debug.Log("HIGHLIGHTED");
				m_selectionRing.SetActive(true);	
			}
		}
		else
		{
			if(m_selectionRing != null)
			{
				m_selectionRing.SetActive(false);	
			}	
		}
	}
	
	private float m_rotationMultiplier = 1.0f;
	private float m_rotationSpeed = 0.0f;
	private float m_rotation = 0.0f;
	private AdminDrone m_drone = null;
	private Color m_defaultColor = Color.white;
}