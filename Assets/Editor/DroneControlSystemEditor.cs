using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(DroneControlSystem))] 
public class DroneControlSystemEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DroneControlSystem controlSystem = (DroneControlSystem)target;
		
		GameObject droneObject = EditorGUILayout.ObjectField(null, typeof(GameObject), false) as GameObject;
		
		if(droneObject != null)
		{
			m_lastError = string.Empty;
			AdminDrone newDrone = droneObject.GetComponent<AdminDrone>();
			
			if(newDrone != null)
			{
				controlSystem.m_registeredDroneObjects.Add(droneObject);
			}
			else
			{
				m_lastError = "Object \"" + droneObject.name + "\" does not contain an AdminNode component";
			}
		}
		
		m_toDelete.Clear();
		
		if(controlSystem.m_registeredDroneObjects.Count > 0)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			foreach(var drone in controlSystem.m_registeredDroneObjects)
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(drone.name);	
				if(GUILayout.Button("Delete", GUILayout.Width(50)))
				{
					m_toDelete.Add(drone);
				}
				
				GUILayout.EndHorizontal();
			}
			
			GUILayout.EndHorizontal();
		}
		
		if(!string.IsNullOrEmpty(m_lastError))
		{
			GUI.contentColor = Color.red;
			GUILayout.Label(m_lastError);	
			GUI.contentColor = Color.black;
		}
		
		foreach(var deleteDrone in m_toDelete)
		{
			controlSystem.m_registeredDroneObjects.Remove(deleteDrone);
		}
	}
			
	private List<GameObject> m_toDelete = new List<GameObject>();
	private string m_lastError = string.Empty;
}
