using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour 
{
	public void OnSerializeNetworkView (BitStream stream,  NetworkMessageInfo info)
	{
		if (stream.isWriting) 
		{
			var pos = transform.position;
			stream.Serialize( ref pos );
		} 
		else 
		{
			Vector3 pos = new Vector3();
			
			stream.Serialize( ref pos);
			transform.position = pos;
		}
	}
}
