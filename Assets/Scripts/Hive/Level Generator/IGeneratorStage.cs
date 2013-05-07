public interface IGeneratorStage  
{
	void Start();
	void End();
	void UpdateStep();
	void UpdateAll();
	bool StageComplete();
	void SetupGUI();
	void UpdateGUI();
	void UpdateSceneGUI();
	string GetStageName();
}
