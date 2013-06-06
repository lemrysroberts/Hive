using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScreenSpaceScale : MonoBehaviour 
{
	public Vector2 InitialScale = new Vector2(1.0f, 1.0f);
	
	// Update is called once per frame
	void Update () 
	{
		transform.localScale = new Vector3(Camera.main.orthographicSize * InitialScale.x, Camera.main.orthographicSize * InitialScale.y, 1.0f);
		
#if UNITY_EDITOR
		// The scene camera can be null if the scene view doesn't have focus.
		if(Camera.current != null)
		{
			transform.localScale = new Vector3(Camera.main.orthographicSize * InitialScale.x, Camera.main.orthographicSize * InitialScale.y, 1.0f);
		}
#endif
	}
}
