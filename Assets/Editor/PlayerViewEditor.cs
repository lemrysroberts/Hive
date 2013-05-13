using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(AgentCamera))] 
public class PlayerViewEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		AgentCamera view = (AgentCamera)target;
		
		view.ShowCandidateRays = GUILayout.Toggle(view.ShowCandidateRays, "Candidate Rays");
		view.ShowSucceededRays = GUILayout.Toggle(view.ShowSucceededRays, "Succeeded Rays");
		view.ShowExtrusionRays = GUILayout.Toggle(view.ShowExtrusionRays, "Extrusion Rays");
		view.collisionLayer = EditorGUILayout.LayerField("Collision Layer", view.collisionLayer);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Nudge Magnitude", GUILayout.Width(100));
		GUILayout.Label(view.m_nudgeMagnitude.ToString("0.00"), GUILayout.Width(40));
		view.m_nudgeMagnitude = GUILayout.HorizontalSlider(view.m_nudgeMagnitude, 0.001f, 0.999f);
		GUILayout.EndHorizontal();
	}
}
