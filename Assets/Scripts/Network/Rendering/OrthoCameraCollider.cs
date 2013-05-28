using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class OrthoCameraCollider : MonoBehaviour 
{
	void Start () 
	{
		Camera camera = GetComponent<Camera>();
		BoxCollider collider = GetComponent<BoxCollider>();
		
		float height = 2.0f * camera.orthographicSize;
		collider.size = new Vector3(height * camera.aspect, height, 30.0f);
	}
}
