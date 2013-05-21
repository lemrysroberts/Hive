using UnityEngine;
using System.Collections;

public class AdminDoor : MonoBehaviour 
{
	public Material OpenMaterial;
	public Material ClosedMaterial;
		
		
	public Door m_door = null;
	public MeshRenderer m_renderer = null;
	

	// Use this for initialization
	void Start () 
	{
		m_door = GetComponent<Door>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_door.m_opening != m_lastOpening)
		{
			m_lastOpening = m_door.m_opening;	
			if(m_door.m_opening)
			{
				m_renderer.material = OpenMaterial;
			}
			else
			{
				m_renderer.material = ClosedMaterial;
			}
		}
		else
		{
			
		}
		//renderer.material = 
	}
			
	private bool m_lastOpening = false;
	private bool m_lastOpen = false;
}
