using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Level))]
public class LevelGen : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		m_level = GetComponent<Level>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private Level m_level;
}
