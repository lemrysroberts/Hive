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
			var rot = transform.localRotation;
			stream.Serialize( ref pos );
			stream.Serialize( ref rot );
			
		} else {
			
			Vector3 pos = new Vector3();
			Quaternion rot = new Quaternion();
			
			stream.Serialize( ref pos);
			stream.Serialize( ref rot);
			
			transform.position = pos;
			transform.localRotation = rot;
		}
	}
}
