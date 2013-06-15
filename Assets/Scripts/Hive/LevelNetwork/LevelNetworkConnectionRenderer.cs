using UnityEngine;
using System.Collections;

public class LevelNetworkConnectionRenderer : MonoBehaviour 
{
	public LevelNetworkConnection m_connection = null;
	
	private LineRenderer m_renderer = null;
	private float alpha = 0.2f;
	
	void Start () 
	{
		m_connection.startNode.NodeAvailable += new System.Action(NodeStateChanged);
		m_connection.endNode.NodeAvailable += new System.Action(NodeStateChanged);
		m_connection.startNode.NodeClaimed += new System.Action(NodeStateChanged);
		m_connection.endNode.NodeClaimed += new System.Action(NodeStateChanged);
		m_connection.startNode.NodeRescinded += new System.Action(NodeStateChanged);
		m_connection.endNode.NodeRescinded += new System.Action(NodeStateChanged);
		m_connection.startNode.NodeUnavailable  += new System.Action(NodeStateChanged);
		m_connection.endNode.NodeUnavailable  += new System.Action(NodeStateChanged);
		
		m_renderer = GetComponent<LineRenderer>();
		
		if(m_connection != null)
		{
			m_lastStartHeat = m_connection.startNode.Heat;
			m_lastEndHeat 	= m_connection.endNode.Heat;
			
			m_renderer.material.SetVector("_StartColor", new Vector4(1.0f, 1.0f - m_lastStartHeat, 1.0f - m_lastStartHeat, alpha));
			m_renderer.material.SetVector("_EndColor", new Vector4(1.0f, 1.0f - m_lastEndHeat, 1.0f - m_lastEndHeat, alpha));
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
		/*
		if( m_lastStartHeat != m_connection.startNode.Heat || 
			m_lastEndHeat != m_connection.endNode.Heat )
		{
			m_lastStartHeat = m_connection.startNode.Heat;
			m_lastEndHeat = m_connection.endNode.Heat;
			
			m_renderer.material.SetVector("_StartColor", new Vector4(1.0f, 1.0f - m_lastStartHeat, 1.0f - m_lastStartHeat, Mathf.Max(m_lastStartHeat, alpha)));
			m_renderer.material.SetVector("_EndColor", new Vector4(1.0f, 1.0f - m_lastEndHeat, 1.0f - m_lastEndHeat, Mathf.Max(m_lastEndHeat, alpha)));
		}
		*/
	}
	
	private void NodeStateChanged()
	{
		if(m_connection.startNode.Available && m_connection.endNode.Available)
		{
			alpha = 0.6f;	
			
			if(m_connection.startNode.Claimed && m_connection.endNode.Claimed)
			{
				m_renderer.material.SetVector("_StartColor", new Vector4(0.0f, 1.0f, 0.0f, alpha));
				m_renderer.material.SetVector("_EndColor", new Vector4(0.0f, 1.0f , 0.0f, alpha));	
			}
			
		}
		else
		{
			alpha = 0.2f;	
			m_renderer.material.SetVector("_StartColor", new Vector4(1.0f, 1.0f - m_lastStartHeat, 1.0f - m_lastStartHeat, Mathf.Max(m_lastStartHeat, alpha)));
			m_renderer.material.SetVector("_EndColor", new Vector4(1.0f, 1.0f - m_lastEndHeat, 1.0f - m_lastEndHeat, Mathf.Max(m_lastEndHeat, alpha)));
		}
		
		
	}
	
	private bool m_active = false;
	
	private float m_lastStartHeat = 0.0f;
	private float m_lastEndHeat = 0.0f;
	
}
