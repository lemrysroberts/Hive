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

public class CreateGoals : IGeneratorStage
{
	public CreateGoals(Level level)
	{
		m_level = level;	
	}
	
	public void Start(){ }
	public void End(){ }
	
	public void UpdateStep()
	{
		// Just do a lazy diagonal search for the start point.
		int step = 0;
		bool foundStart = false;
		while(step < m_level.Width && step < m_level.Height && !foundStart)
		{
			if(!m_level.TileBlocked(step, step))
			{
				foundStart = true;
				m_level.PlayerSpawnPoint = new Vector2(step, step);
			}
			
			step++;	
		}
		
		step = Mathf.Min(m_level.Width - 1, m_level.Height - 1);
		bool foundGoal = false;
		
		LevelObjectDatabase database = GameObject.FindObjectOfType(typeof(LevelObjectDatabase)) as LevelObjectDatabase;
		
		if(database != null)
		{
			
			while(step > 0 && !foundGoal)
			{
				if(!m_level.TileBlocked(step, step))
				{
					foundGoal = true;
					m_level.GoalItemSpawnPoint = new Vector2(step, step);
					LevelObject itemObject = database.GetObject("goalitem");
					if(itemObject != null)
					{
						itemObject.Position = new Vector3(step, step, -1);
						m_level.AddLevelObject(itemObject);
					}
				}
				
				step--;	
			} 
			
			step = Mathf.Min(m_level.Width - 1, m_level.Height - 1);
			foundGoal = false;
			while(step > 0 && !foundGoal)
			{
				if(!m_level.TileBlocked((int)m_level.PlayerSpawnPoint.x + 1, step))
				{
					foundGoal = true;
					LevelObject areaObject = database.GetObject("goalarea");
					if(areaObject != null)
					{
						areaObject.Position = new Vector3(m_level.PlayerSpawnPoint.x + 0.5f, step + 0.5f, -1);
						m_level.AddLevelObject(areaObject);
					}
					
				}
				step--;
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
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, "Goals");
		
		if(m_showFoldout)
		{
		}
#endif
	}
	
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Create Goals"; }
	
	private Level m_level;
	private static bool m_showFoldout = false;
}
