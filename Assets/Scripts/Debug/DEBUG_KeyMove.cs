using UnityEngine;
using System.Collections;

public class DEBUG_KeyMove : MonoBehaviour {
	
	public float MoveSpeed = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + (0.01f * MoveSpeed), transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - (0.01f * MoveSpeed), transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.position = new Vector3(transform.position.x - (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.position = new Vector3(transform.position.x + (0.01f  * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetMouseButtonDown(1))
		{
			m_dragging = true;
			
			m_lastMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}
		else if(Input.GetMouseButtonUp(1))
		{
			m_dragging = false;	
		}
		
		if(m_dragging && Input.GetMouseButton(1))
		{
			Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);	
			Vector2 diff = m_lastMousePos - newPosition;
			
			float dragSpeed = 0.1f;
			transform.position = new Vector3(transform.position.x + (diff.x * dragSpeed), transform.position.y, transform.position.z + (diff.y * dragSpeed));
			
			
			m_lastMousePos = newPosition;
		}
	}
	
	private Vector2 m_lastMousePos = new Vector2(0.0f, 0.0f);
	private bool m_dragging = false;
}
