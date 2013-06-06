using UnityEngine;
using System.Collections;

public class ClaimProgressBar : MonoBehaviour 
{
	public float progress = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.localScale = new Vector3(progress, transform.localScale.y, transform.localScale.z);
	}
}
