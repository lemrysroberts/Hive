#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class CreateLevelObjects : IGeneratorStage
{
	public CreateLevelObjects(Level level)
	{
		m_level = level;	
	}
	
	public void Start(){ }
	public void End(){ }
	
	public void UpdateStep()
	{
		m_level.CreateLevelObjects();
	}
	
	public void UpdateAll()
	{
		UpdateStep();
	}
	
	public bool StageComplete() { return true; }
	public void SetupGUI(){ }
	public void UpdateGUI(){ }
	public void UpdateSceneGUI(){ }
	public string GetStageName(){ return "Create Level Objects"; }
	
	private Level m_level;
}
