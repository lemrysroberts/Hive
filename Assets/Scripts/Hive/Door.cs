using UnityEngine;
using System.Xml;
using System.Collections;

public class Door : SensorTarget
{
	public GameObject MeshObject = null;
	public float DoorSpeed = 0.1f;
	public bool Locked = false;
	
	public void Start()
	{
		m_originalTransform = transform.position;
		
		Vector3 rotated = (MeshObject.transform.rotation * Vector3.right) * transform.lossyScale.x;
		m_openTransform = (MeshObject.transform.position - rotated) ;
		
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
			MeshObject.transform.position = Vector3.Lerp(m_originalTransform, m_openTransform, m_openProgress);
		}
		else if((!m_opening || Locked) && m_openProgress > 0.0f)
		{
			m_openProgress -= DoorSpeed;
			if(m_openProgress < 0.0f)
			{
				m_openProgress = 0.0f;	
			}
			MeshObject.transform.position = Vector3.Lerp(m_originalTransform, m_openTransform, m_openProgress);
		}
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
		} else {
	
			bool opening = false;
			// Receiving
			stream.Serialize( ref opening);
			if(opening )
			{
				Debug.LogError("DOOR OPENING");	
			}
			// ... do something meaningful with the received variable
		}
	}
	
	private Vector3 m_originalTransform;
	private Vector3 m_openTransform;
	
	private int m_activationCount = 0;
	private float m_openProgress = 0.0f;
	private bool m_opening = false;
}
