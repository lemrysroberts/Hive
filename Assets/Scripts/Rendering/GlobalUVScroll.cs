using UnityEngine;
using System.Collections;

public class GlobalUVScroll : MonoBehaviour 
{
	public float ScrollSpeed = 0.005f;
	
	
	// Update is called once per frame
	float current = 0.0f;
	void FixedUpdate () 
	{
		current += ScrollSpeed;
		Vector4 texOffset = new Vector4(current, current, 0.0f, 0.0f);
		Shader.SetGlobalVector("_uvScroll", texOffset);
	}
}
