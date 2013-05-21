using UnityEngine;
using System.Xml;
using System.Collections;

public class Door : SensorTarget
{
	
	public float DoorSpeed = 0.1f;
	public bool Locked = false;
	
	public void Start()
	{
	}
	
	public override void SensorActivate()
	{
		m_activationCount++;
		if(m_activationCount == 1)
		{
			Open();	
		}
	}
	
	public override void SensorDeactivate()
	{
		m_activationCount--;
		if(m_activationCount == 0)
		{
			Close ();	
		}
	}
	
	public void Update()
	{
		if(m_opening && m_openProgress < 1.0f && !Locked)
		{
			m_openProgress += DoorSpeed;
			if(m_openProgress > 1.0f)
			{
				m_openProgress = 1.0f;	
			}
			
		}
		else if((!m_opening || Locked) && m_openProgress > 0.0f)
		{
			m_openProgress -= DoorSpeed;
			if(m_openProgress < 0.0f)
			{
				m_openProgress = 0.0f;	
			}
		}
		
		if(m_other)
		{
			networkView.RPC("RequestClose", RPCMode.Others);
			m_other = false;
		}
	}
	
	[RPC]
	private void RequestClose()
	{
		Debug.Log("Close Requested");
	}
	
	private void Open()
	{
		m_opening = true;
	}
	
	private void Close()
	{
		m_opening = false;
	}
	
	public void OnSerializeNetworkView (BitStream stream,  NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			// Sending
				
			stream.Serialize (ref m_opening);
			stream.Serialize(ref m_other);
		} else {
	
			stream.Serialize( ref m_opening);
			stream.Serialize( ref m_other);
			
			if(m_other )
			{
				Debug.LogError("DOOR thing");	
			}
			// ... do something meaningful with the received variable
		}
	}
	
	
	
	public int m_activationCount = 0;
	public float m_openProgress = 0.0f;
	public bool m_opening = false;
	public bool m_other = false;
}
