using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnSerializeNetworkView (BitStream stream,  NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			// Sending
			var pos = transform.position;
			stream.Serialize( ref pos );
			
		} else {
			
			Vector3 pos = new Vector3();
			
			stream.Serialize( ref pos);
			transform.position = pos;
		}
	}
}
