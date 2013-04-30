/// <summary>
/// Kinematic key move.
/// 
/// Key-movement class that is the opposite of a kinematic controller. What is wrong with my brain.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class KinematicKeyMove : MonoBehaviour 
{
	public float MoveSpeed = 200f;
	public float TurnSpeed = 2.0f;
	public bool DirectControl = true;
	
	Rigidbody m_controller = null;
	
	// Use this for initialization
	void Start () 
	{
		m_controller = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(DirectControl)
		{
			UpdateControls();	
		}
		else
		{
			UpdateDodgyControls();	
		}
	}
	
	public void UpdateDodgyControls()
	{
		m_controller.velocity = new Vector3(0.0f, 0.0f, 0.0f);
		if(Input.GetKey(KeyCode.UpArrow) )
		{
			
			m_controller.AddForce(transform.rotation * (Vector3.up * MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			m_controller.AddForce(transform.rotation * (Vector3.up * -MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(0.0f, 0.0f, TurnSpeed);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(0.0f, 0.0f, -TurnSpeed);
		}
	}
	
	public void UpdateControls()
	{
		m_controller.velocity = new Vector3(0.0f, 0.0f, 0.0f);
		if(Input.GetKey(KeyCode.UpArrow) )
		{
			m_controller.AddForce(Vector3.up * MoveSpeed);
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			
			m_controller.AddForce(Vector3.up * -MoveSpeed);
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			m_controller.AddForce(Vector3.right * -MoveSpeed);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			m_controller.AddForce(Vector3.right * MoveSpeed);
		}
	}
}
