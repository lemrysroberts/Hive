using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Level))] 
public class LayoutEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Level level = target as Level;
		
		m_editMode = EditorGUILayout.Toggle("Edit", m_editMode);
		
		m_showSectionCounts = EditorGUILayout.Foldout(m_showSectionCounts, "Section Counts");	
				
		if(m_showSectionCounts)
		{
			level.SectionCountX = EditorGUILayout.IntField("X", level.SectionCountX);
			level.SectionCountY = EditorGUILayout.IntField("Y", level.SectionCountY);
		}
		
		level.m_npcObject = EditorGUILayout.ObjectField(level.m_npcObject, typeof(GameObject), true) as GameObject;
		
		if(GUILayout.Button("Reload"))
		{
			level.Reload(false);
			HandleUtility.Repaint();
		}
		
		if(GUILayout.Button("Rebuild all sections"))
		{
			level.RebuildAllSections();
			HandleUtility.Repaint();
		}
		
		if(GUILayout.Button("Save"))
		{
			string levelFile = EditorUtility.SaveFilePanel("Save Level", "Levels", "newlevel", "xml");
			level.Save(levelFile);
		}
		
		if(GUILayout.Button("Load"))
		{
			string levelFile = EditorUtility.OpenFilePanel("Load Level", "Levels", "xml");
			level.Load(levelFile);
		}
		
		if(GUILayout.Button("Rebuild AI-Graph"))
		{
			level.RebuildAIGraph();	
		}
		
		if(GUILayout.Button("Rebuild Colliders"))
		{
			level.RebuildColliders();	
		}
		
		if(GUILayout.Button("Test Routefinder"))
		{
			level.TestRoutefinder();
			EditorUtility.SetDirty(level);
		}
		
		if(GUILayout.Button("Tile Editor"))
		{
			EditorWindow.GetWindow(typeof(TileEditor));	
		}
		
		m_showDebugRenderOptions = EditorGUILayout.Foldout(m_showDebugRenderOptions, "Debug Rendering");	
		if(m_showDebugRenderOptions)
		{
			bool renderColliders = EditorGUILayout.Toggle("Collider Edge Data", level.m_renderColliders);
			bool renderNodeGraph = EditorGUILayout.Toggle("Node Graph", level.m_renderNodeGraph);
			
			if(renderColliders != level.m_renderColliders || renderNodeGraph != level.m_renderNodeGraph)
			{
				level.m_renderColliders = renderColliders;
				level.m_renderNodeGraph = renderNodeGraph;
				
				HandleUtility.Repaint();
			}
		}
	}
	
	public void OnSceneGUI()
	{
		Level level = target as Level;
		
		if(m_editMode)
		{
			Event e = Event.current;
			Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));
			
			Plane zeroPlane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f));
			
			float hit;
			if(zeroPlane.Raycast(ray, out hit))
			{
				var hitLocation = ray.GetPoint(hit);
					
				Vector3 targetPos = new Vector3(hitLocation.x, hitLocation.y, 0.0f);
				if(e.button == 0 && e.type == EventType.MouseDrag)
				{
					PaintTile(new Vector2(targetPos.x, targetPos.y));
				}
			
				Handles.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), targetPos);
				HandleUtility.Repaint();
			}
			
			// These rules define which editor events are consumed.
			if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown ) && (e.button == 0 || e.button == 1) ||  (e.type == EventType.Layout))
	        {
				
				e.Use();
			    {
			       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
			    }
			}	
		}
		
		if(level.m_renderColliders)
		{
			foreach(var section in level.m_sections)
			{
				Handles.Label(section.Origin, "Edges: " + section.m_edges.Count);
			}
		}
	}
	
	void PaintTile(Vector2 position)
	{
		Level level = target as Level;
		
		if(position.x > 0.0f && position.y > 0.0f && position.x < (level.SectionCountX * Level.m_sectionSize) && position.y < (level.SectionCountY * Level.m_sectionSize))
		{
			level.SetTileID((int)position.x, (int)position.y, TileManager.Instance.SelectedTile.ID, true);
		}
		
	}
		
	private bool m_showSectionCounts = true;
	private bool m_showDebugRenderOptions = false;
	private bool m_editMode = false;
}
