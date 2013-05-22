using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ViewDisable))] 
public class ViewDisableEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		ViewDisable viewDisable = (ViewDisable)target;
		
		m_showAgentFoldout = EditorGUILayout.Foldout(m_showAgentFoldout, "Agent Disabled Components");
		
		if(m_showAgentFoldout)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Add Script", GUILayout.Width(100));
			MonoBehaviour newBehaviour =  EditorGUILayout.ObjectField(null, typeof(MonoBehaviour), true) as MonoBehaviour;
			GUILayout.EndHorizontal();
			
			if(newBehaviour != null)
			{
				Debug.Log(newBehaviour.GetType());
				viewDisable.AgentDisabledItems.Add(newBehaviour);	
			}
			
			List<MonoBehaviour> toDelete = new List<MonoBehaviour>();
			
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			
			foreach(var behaviour in viewDisable.AgentDisabledItems)
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(behaviour.GetType().ToString());
				if(GUILayout.Button("remove", GUILayout.Width(80)))
				{
					toDelete.Add(behaviour);	
				}
				GUILayout.EndHorizontal();
			}
			
			foreach(var behaviour in toDelete)
			{
				viewDisable.AgentDisabledItems.Remove(behaviour);	
			}
			GUILayout.EndVertical();
		}
		
		m_showAdminFoldout = EditorGUILayout.Foldout(m_showAdminFoldout, "Admin Disabled Components");
		
		if(m_showAdminFoldout)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Add Script", GUILayout.Width(100));
			MonoBehaviour newBehaviour =  EditorGUILayout.ObjectField(null, typeof(MonoBehaviour), true) as MonoBehaviour;
			GUILayout.EndHorizontal();
			
			if(newBehaviour != null)
			{
				viewDisable.AdminDisabledItems.Add(newBehaviour);	
			}
			
			List<MonoBehaviour> toDelete = new List<MonoBehaviour>();
			foreach(var behaviour in viewDisable.AdminDisabledItems)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(behaviour.GetType().ToString());
				if(GUILayout.Button("remove", GUILayout.Width(80)))
				{
					toDelete.Add(behaviour);	
				}
				GUILayout.EndHorizontal();
			}
			
			foreach(var behaviour in toDelete)
			{
				viewDisable.AdminDisabledItems.Remove(behaviour);	
			}
			GUILayout.EndVertical();
		}
		
	}
	
	private string m_object = string.Empty;
	private bool m_showAgentFoldout = false;
	private bool m_showAdminFoldout = false;
}
