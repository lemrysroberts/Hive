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
	}
	
	public void UpdateAll()
	{
		UpdateStep();
	}
	
	public bool StageComplete() { return true; }
	public void SetupGUI(){ }
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Create Goals"; }
	
	private Level m_level;
}
