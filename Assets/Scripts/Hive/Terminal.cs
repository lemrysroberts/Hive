using UnityEngine;
using System.Collections;

public class Terminal : MonoBehaviour 
{
	// System.Action is just a default delegate with no return-type or arguments
	public event System.Action HackedStateChanged;
	
	public void OnSerializeNetworkView (BitStream stream,  NetworkMessageInfo info)
	{
		if (stream.isWriting) 
		{
			stream.Serialize (ref m_hacked);
		}
		else 
		{
			bool hacked = m_hacked;
			
			stream.Serialize( ref hacked);
			
			if(hacked != m_hacked)
			{
				m_hacked = hacked;
				if(HackedStateChanged != null)
				{
					HackedStateChanged();	
				}
			}
		}
	}
	
	public bool m_hacked = false;

}
