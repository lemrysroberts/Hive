#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class LevelGen  
{
	public LevelGen(Level level)
	{
		m_level = level;
		m_stages.Add(new BinaryCorridors(m_level));
		m_stages.Add(new RoomBuilder(level));
		m_stages.Add(new DoorPlacement(level));
		m_stages.Add(new CreateLevelObjects(level));
	}
	
	public void GenerateLevel(int seed, bool stepUpdate)
	{
		m_generationComplete = false;
		m_currentStageIndex = 0;
				
		if(seed >= 0)
		{
			Random.seed = seed;	
		}
		
		m_level.Reload(true);
		
		if(!stepUpdate)
		{
			foreach(var stage in m_stages)
			{
				stage.Start();
				stage.UpdateAll();
				stage.End();
			}
			m_level.RebuildColliders();
			m_level.RebuildAIGraph();
		}
		else 
		{
			if(m_currentStageIndex < m_stages.Count)
			{
				m_stages[m_currentStageIndex].Start();
			}
			else
			{
				m_generationComplete = true;	
			}
		}
		
		m_level.RebuildAllSections();
	}
	
#if UNITY_EDITOR
	public void UpdateStep()
	{
		// This is just here for safety.
		if(m_currentStageIndex == m_stages.Count)
		{
			m_generationComplete = true;
			return;
		}
			
		m_stages[m_currentStageIndex].UpdateStep();
		
		if(m_stages[m_currentStageIndex].StageComplete())
		{
			m_stages[m_currentStageIndex].End();
			m_currentStageIndex++;
			
			if(m_currentStageIndex == m_stages.Count)
			{
				m_generationComplete = true;
				return;
			}
			
			m_stages[m_currentStageIndex].Start();
		}
	}
	
	public void UpdateStage()
	{
		int currentStage = m_currentStageIndex;
		while(m_currentStageIndex == currentStage && !m_generationComplete)
		{
			UpdateStep();	
		}
	}

#endif
	
	public bool GenerationComplete
	{
		get { return m_generationComplete; }	
	}
	
	public IGeneratorStage CurrentStage
	{
		get { return m_currentStageIndex < m_stages.Count ? m_stages[m_currentStageIndex] : null; }	
	}
	
	public List<IGeneratorStage> Stages
	{
		get { return m_stages; }	
	}
	
	private List<IGeneratorStage> m_stages = new List<IGeneratorStage>();
	private int m_currentStageIndex = 0;
	private Level m_level;
	private bool m_generationComplete = false;
}
 