using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Level))] 
public class LevelEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Level level = target as Level;
		
		if(m_generator == null)
		{
			m_generator = new LevelGenerator(level);	
		}
		
		if(DrawGenerationActiveGUI())
		{
			return;	
		}
		
		m_editMode = EditorGUILayout.Toggle("Edit", m_editMode);
		
		m_showSectionCounts = EditorGUILayout.Foldout(m_showSectionCounts, "Section Counts");	
				
		if(m_showSectionCounts)
		{
			level.SectionCountX = EditorGUILayout.IntField("X", level.SectionCountX);
			level.SectionCountY = EditorGUILayout.IntField("Y", level.SectionCountY);
		}

		if(GUILayout.Button("Rebuild all sections"))
		{
			level.RebuildAllSections();
			EditorUtility.SetDirty(level);
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
			bool renderColliders 	= EditorGUILayout.Toggle("Collider Edge Data", level.m_renderColliders);
			bool renderNodeGraph 	= EditorGUILayout.Toggle("Node Graph", level.m_renderNodeGraph);
			bool renderRooms		= EditorGUILayout.Toggle("Rooms", level.m_renderRooms);
			
			if( renderColliders != level.m_renderColliders || 
				renderNodeGraph != level.m_renderNodeGraph ||
				renderRooms != level.m_renderRooms)
			{
				level.m_renderColliders = renderColliders;
				level.m_renderNodeGraph = renderNodeGraph;
				level.m_renderRooms = renderRooms;
				
				EditorUtility.SetDirty(level);
			}
		}
		GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
		m_showLevelGenOptions = EditorGUILayout.Foldout(m_showLevelGenOptions, "Level Generator");
		
		if(m_showLevelGenOptions)
		{
			DrawGeneratorGUI(level);
		}
	}
	
	public void OnSceneGUI()
	{
		Level level = target as Level;
		
		if(m_generatingLevel && m_generator != null)
		{
			if(m_generator.CurrentStage != null)
			{
				m_generator.CurrentStage.UpdateSceneGUI();
			}
			
			return;
		}
		
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
	
	/// <summary>
	/// Draws the GUI for the level-generator.
	/// </summary>
	private void DrawGeneratorGUI(Level level)
	{
		m_rebuildColliders 	= GUILayout.Toggle(m_rebuildColliders, "Rebuild Colliders On Completion");
		m_rebuildNodeGraph 	= GUILayout.Toggle(m_rebuildNodeGraph, "Rebuild AI Node-Graph On Completion");
		m_useTestSeed 		= GUILayout.Toggle(m_useTestSeed, "Use Custom Seed");
		
		if(m_useTestSeed)
		{
			m_generatorTestSeed = EditorGUILayout.IntField("Custom Seed", m_generatorTestSeed);
		}
		
		m_stepGenerate 		= GUILayout.Toggle(m_stepGenerate, "Step Generate");
		
		GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
		
		GUILayout.Label("Stages");
		foreach(var stage in m_generator.Stages)
		{
			stage.SetupGUI();
		}
		
		GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
		
		if(GUILayout.Button("Generate Level"))
		{
			if(EditorUtility.DisplayDialog("Generate New Level?", "This will reset the current level. Continue?", "Ok", "Cancel"))
			{
				m_generator = new LevelGenerator(level);
				m_generator.GenerateLevel(m_useTestSeed ? m_generatorTestSeed : -1, true);
				m_generatingLevel = true;
				m_stageToAdvance = m_stepGenerate ? null : m_generator.CurrentStage;
			}
		}
	}
	
	/// <summary>
	/// Draws the GUI when the generator is running in some form.
	/// </summary>
	/// <returns>
	/// True if generation UI is active. False otherwise
	/// </returns>
	private bool DrawGenerationActiveGUI()
	{
		Level level = target as Level;
		if(m_generatingLevel && m_generator != null)
		{
			// Cancel
			if(m_stageToAdvance != null)
			{
				if(GUILayout.Button("Cancel"))
				{
					m_generatingLevel = false;	
				}
			}
			
			// More stages to process
			if(m_stageToAdvance != null)
			{
				m_generator.UpdateStep();
				level.RebuildAllSections();	
				
				EditorUtility.SetDirty(level);
				
				if(m_generator.CurrentStage != m_stageToAdvance)
				{
					if(m_stepGenerate)
					{
						m_stageToAdvance = null;
					}
					else
					{
						m_stageToAdvance = m_generator.CurrentStage;
					}
					m_generatingLevel = !m_generator.GenerationComplete;
					if(!m_generatingLevel)
					{
						GenerationComplete(level);	
					}
					
					return true;
				}
			}
			
			if(m_stepGenerate && m_stageToAdvance == null)
			{
				if(GUILayout.Button("Step"))
				{
					m_generator.UpdateStep();
					m_generatingLevel = !m_generator.GenerationComplete;
					
					if(!m_generatingLevel)
					{
						GenerationComplete(level);	
					}
					
					level.RebuildAllSections();	
					EditorUtility.SetDirty(level);
				}
				
				if(GUILayout.Button("Advance Stage"))
				{
					m_stageToAdvance = m_generator.CurrentStage;
				}
			}
			
			if(m_generator.CurrentStage != null)
			{
				GUILayout.Label("Active Stage: " + m_generator.CurrentStage.GetStageName());
				m_generator.CurrentStage.UpdateGUI();
			}
			
			return true;
		}
		return false;
	}
	
	private void GenerationComplete(Level level)
	{
		if(m_rebuildColliders) { level.RebuildColliders(); }
		if(m_rebuildNodeGraph) { level.RebuildAIGraph(); }
	}
		
	private bool m_showSectionCounts 		= true;
	private bool m_showDebugRenderOptions 	= false;
	private bool m_editMode 				= false;
	
	// Gubbins for the level-generator.
	private bool m_showLevelGenOptions 				= false;
	private int m_generatorTestSeed 				= 0;
	private bool m_useTestSeed 						= false;
	private static bool m_rebuildColliders			= true;
	private static bool m_rebuildNodeGraph			= true;
	private static bool m_stepGenerate 				= false;
	private static bool m_generatingLevel 			= false;
	private static LevelGenerator m_generator 			= null;
	private static IGeneratorStage m_stageToAdvance = null;
}
