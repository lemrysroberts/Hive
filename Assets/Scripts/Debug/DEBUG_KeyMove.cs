using UnityEngine;
using System.Collections;

public class DEBUG_KeyMove : MonoBehaviour {
	
	public float MoveSpeed = 1.0f;
	public float ZoomSpeed = 15.0f;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		/*
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
	*/	
		if(Input.GetMouseButtonDown(1))
		{
			m_dragging = true;
			
			m_lastMousePos = Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
			Debug.Log("Mouse down");
		}
		else if(Input.GetMouseButtonUp(1))
		{
			m_dragging = false;	
			Debug.Log("Mouse up");
		}
	
		if(m_dragging && Input.GetMouseButton(1))
		{
			Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);	
			
			
		//	Debug.Log("Test: " + diff.x + ", " + diff.y);
			Vector3 camPosition = Camera.mainCamera.ScreenToWorldPoint(newPosition);
			
			
			//Debug.Log("New pos: " + camPosition.x + ", " + camPosition.y);
			//Debug.Log("Old pos: " + transform.position.x + ", " + transform.position.y);
			Vector3 newPos =  transform.position - ( camPosition - m_lastMousePos);
			transform.position =  new Vector3(newPos.x, newPos.y, transform.position.z);
			
			float dragSpeed = 1.0f;
			//transform.position = new Vector3(transform.position.x + (diff.x * dragSpeed), transform.position.y  + (diff.y * dragSpeed), transform.position.z);
			
			
			m_lastMousePos = Camera.mainCamera.ScreenToWorldPoint(newPosition);
		}
		
		m_zoom -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
		m_zoom = Mathf.Min(m_zoom, 50.0f);
		m_zoom = Mathf.Max(m_zoom, 9.0f);
		
		camera.orthographicSize =  m_zoom;
	}
	
	private Vector3 m_lastMousePos = new Vector2(0.0f, 0.0f);
	private bool m_dragging = false;
	private float m_zoom = 5.0f;
	
	
}
