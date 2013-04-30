/// <summary>
/// Misc info.
/// 
/// This class really just exists for my entertainment and education.
/// </summary>

using UnityEngine;
using System.Collections;

public class MiscInfo : MonoBehaviour 
{
	public LayerMask HitLayers;

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonUp(0))
		{
			RaycastHit hit;
        	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
			m_lastRay = ray;
        	if (Physics.Raycast(ray, out hit, 100.0f, HitLayers.value)) 
			{
				m_hitObject = hit.collider.gameObject;
       		}
			
		}
		
		if(Input.GetMouseButtonUp(1))
		{
			m_hitObject = null;	
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(m_lastRay);	
	}
	
	void OnGUI()
	{
		if(m_hitObject != null)
		{
			Vector3 altVec = m_hitObject.transform.position;
			var point = Camera.mainCamera.WorldToScreenPoint(altVec);
			point = GUIUtility.ScreenToGUIPoint(point);
			
			GUILayout.BeginArea(new Rect(point.x + m_objectOffset.x, (Screen.height - point.y) + m_objectOffset.y, 200.0f, 200.0f), (GUIStyle)("Box"));
			GUILayout.Box(m_hitObject.name);
			
			Component[] components = m_hitObject.GetComponents<Component>();
			
			GUILayout.BeginVertical((GUIStyle)("Box"));
			m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
			
			foreach(var current in components)
			{
				GUILayout.Label(current.GetType().ToString());	
			}
			
			GUILayout.EndScrollView();
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
		}
	}
	
	private Vector2 m_objectOffset = new Vector2(20.0f, 20.0f);
	private GameObject m_hitObject = null;	
	private Ray m_lastRay;
	private Vector2 m_scrollPos;
}
