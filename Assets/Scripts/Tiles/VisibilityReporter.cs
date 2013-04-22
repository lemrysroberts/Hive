using UnityEngine;
using System.Collections;

public interface IVisibilityReceiver
{
	void ElementVisible();
	void ElementInvisible();
}

public class VisibilityReporter : MonoBehaviour 
{
	[SerializeField]
	private IVisibilityReceiver m_receiver = null;
	
	public void RegisterReceiver(IVisibilityReceiver receiver)
	{
		m_receiver = receiver;
	}
		
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "MainCamera" && m_receiver != null)
		{
			m_receiver.ElementVisible();
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "MainCamera" && m_receiver != null)
		{
			m_receiver.ElementInvisible();	
		}
	}
}
