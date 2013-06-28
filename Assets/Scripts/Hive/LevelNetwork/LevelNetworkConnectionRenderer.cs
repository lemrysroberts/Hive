using UnityEngine;
using System.Collections;

public class LevelNetworkConnectionRenderer : MonoBehaviour 
{
	public Color ColdColor = Color.white;
	public Color HotColor = Color.red;
	public float ScaleFactor = 1.0f;
	
	public LevelNetworkConnection m_connection = null;
	
	private LineRenderer m_renderer = null;
	
	void Start () 
	{
		m_renderer = GetComponent<LineRenderer>();
		
		if(m_connection != null)
		{
			m_lastStartHeat = m_connection.startNode.Heat;
			m_lastEndHeat 	= m_connection.endNode.Heat;
			
			m_renderer.material.SetVector("_StartColor", Vector4.Lerp(ColdColor, HotColor, m_lastStartHeat));
			m_renderer.material.SetVector("_EndColor", Vector4.Lerp(ColdColor, HotColor, m_lastEndHeat));
		}	
	}
	
	// Update is called once per frame
	// TODO: Make event-driven rather than updating.
	void Update () 
	{
		if(m_connection == null)
		{
			return;	
		}
		
		m_renderer.SetWidth(Camera.main.orthographicSize * ScaleFactor, Camera.main.orthographicSize * ScaleFactor);
		
		if( m_lastStartHeat != m_connection.startNode.Heat || 
			m_lastEndHeat != m_connection.endNode.Heat )
		{
			
			
			m_lastStartHeat = m_connection.startNode.Heat;
			m_lastEndHeat = m_connection.endNode.Heat;
			
			m_renderer.material.SetVector("_StartColor", Vector4.Lerp(ColdColor, HotColor, m_lastStartHeat));
			m_renderer.material.SetVector("_EndColor", Vector4.Lerp(ColdColor, HotColor, m_lastEndHeat));
		}
		
	}
	
	private void NodeStateChanged()
	{
	
	}
	
	private float m_lastStartHeat = 0.0f;
	private float m_lastEndHeat = 0.0f;
	
}
