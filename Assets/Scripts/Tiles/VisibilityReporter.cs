using UnityEngine;
using System.Collections;

public interface IVisibilityReceiver
{
	void ElementVisible();
	void ElementInvisible();
}

public class VisibilityReporter : MonoBehaviour 
{
	private IVisibilityReceiver m_receiver = null;
	
	public void RegisterReceiver(IVisibilityReceiver receiver)
	{
		m_receiver = receiver;
	}
		
	public void OnBecameVisible()
	{
		if(m_receiver != null)
		{
			m_receiver.ElementVisible();
		}
	}
	
	public void OnBecameInvisible()
	{
		if(m_receiver != null)
		{
			m_receiver.ElementInvisible();	
		}
		
		
	}
}
