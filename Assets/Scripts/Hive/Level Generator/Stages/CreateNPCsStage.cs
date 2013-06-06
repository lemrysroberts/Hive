/// <summary>
/// Create goals.
/// 
/// Placeholder stage for creating simple goals.
/// 
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class CreateNPCsStage : IGeneratorStage
{
	public CreateNPCsStage(Level level)
	{
		m_level = level;	
	}
	
	public void Start(){ }
	public void End(){ }
	
	public void UpdateStep()
	{
		LevelObjectDatabase database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		
		if(database != null)
		{
			for(int i = 0; i < m_npcCount; i++)
			{
				Room currentRoom = m_level.Rooms[i];
				
				LevelObject npcObject = database.GetObject("npc");
				npcObject.Position = new Vector3(currentRoom.startX + 0.5f, currentRoom.startY + 0.5f, 0.0f);
				m_level.AddLevelObject(npcObject);
			}
		}
	}
	
	public void UpdateAll()
	{
		UpdateStep();
	}
	
	public bool StageComplete() { return true; }
	public void SetupGUI()
	{
#if UNITY_EDITOR
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, GetStageName());
		
		if(m_showFoldout)
		{
			m_npcCount = EditorGUILayout.IntField("NPC Count", m_npcCount);
		}
#endif
	}
	
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Create NPCs"; }
	
	private Level m_level;
	private static bool m_showFoldout = false;
	private int m_npcCount = 10;
}
