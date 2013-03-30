using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class Tile : MonoBehaviour 
{
	private MeshRenderer m_renderer;
	// Use this for initialization
	void Start () 
	{
		m_renderer = GetComponent<MeshRenderer>();
		transform.position = m_position + new Vector2(0.5f, 0.5f);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public Vector2 m_position = new Vector2();
}
