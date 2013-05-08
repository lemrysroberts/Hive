/// <summary>
/// Should I change this to an abstract class?
/// A lot of its guff is general. Hmm.....
/// </summary>

public interface IGeneratorStage  
{
	void Start();			// Called at the start of the stage. *shock*. Before any updates but after construction.
	void End();				// The opposite of the above. Used to copy finalised data, etc.
	void UpdateStep();		// Updates an individual chunk of the stage. Used in-editor to visualise generation.
	void UpdateAll();		// Runs the stage to completion. 
	bool StageComplete();	// Return whether the stage has finished.
	void SetupGUI();		// Display the inspector-GUI for the stage prior to beginning generation.
	void UpdateGUI();		// Display the inspector-GUI for the stage whilst the stage is being processed.
	void UpdateSceneGUI();	// Display scene-GUI elements while the stage is being processed.
	string GetStageName();	// Returns a string for the stage's name.
}
