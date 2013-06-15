/// <summary>
/// Misc info.
/// 
/// This class really just exists for my entertainment and education.
/// </summary>

using UnityEngine;
using System.Collections;

public class NetworkNodeMenu : MonoBehaviour 
{
	void Update () 
	{
		if(Input.GetMouseButtonUp(0))
		{
        	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
			RaycastHit[] info = Physics.RaycastAll(ray, 100.0f);
			
			foreach(var currentHit in info)
			{
				if(currentHit.collider.gameObject.GetComponent<LevelNetworkSelectableNode>() != null)
				{
					m_hitObject = currentHit.collider.gameObject;
					break;
				}
			}
		}
		
		if(Input.GetMouseButtonUp(1))
		{
			m_hitObject = null;	
		}
	}
	
	void OnGUI()
	{
		return;
		if(m_hitObject != null)
		{
			LevelNetworkNode node = m_hitObject.GetComponent<LevelNetworkSelectableNode>().m_node;
			if(node.Commands.Count > 0 || node.InfoStrings.Count > 0)
			{
				Vector3 altVec = m_hitObject.transform.position;
				var point = Camera.mainCamera.WorldToScreenPoint(altVec);
				point = GUIUtility.ScreenToGUIPoint(point);
				
				GUILayout.BeginArea(new Rect(point.x + m_objectOffset.x, (Screen.height - point.y) + m_objectOffset.y, 200.0f, 200.0f), (GUIStyle)("Box"));
				GUILayout.Box(m_hitObject.name);
				
				
				
				GUILayout.BeginVertical((GUIStyle)("Box"));
				m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
				
				foreach(var current in node.Commands)
				{
					if(GUILayout.Button(current.DisplayName))
					{
						node.IssueCommand(current);
						m_hitObject = null;
						break;
					}
				}
				
				GUILayout.Box("", GUILayout.Height(1));
				
				foreach(var current in node.InfoStrings)
				{
					GUILayout.Label(current);	
				}
				
				GUILayout.EndScrollView();
				
				GUILayout.EndVertical();
				
				GUILayout.EndArea();
			}
		}
	}
	
	private Vector2 m_objectOffset = new Vector2(20.0f, 20.0f);
	private GameObject m_hitObject = null;	
	private Vector2 m_scrollPos;
}
