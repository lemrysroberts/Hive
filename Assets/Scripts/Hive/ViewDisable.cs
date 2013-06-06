using UnityEngine;
using System.Collections.Generic;

public class ViewDisable : MonoBehaviour 
{
	public List<MonoBehaviour> AdminDisabledItems = new List<MonoBehaviour>();
	public List<MonoBehaviour> AgentDisabledItems = new List<MonoBehaviour>();
	
	public bool DisableEntireObjectAdmin = false;
	public bool DisableEntireObjectAgent = false;
	
	void OnEnable()
	{
		WorldView view = GameFlow.Instance.View;
		
		if(view == WorldView.Admin && DisableEntireObjectAdmin)
		{
			gameObject.SetActive(false);	
		}
		
		if(view == WorldView.Agent && DisableEntireObjectAgent)
		{
			gameObject.SetActive(false);
		}
		
		List<MonoBehaviour> objectsToDisable = view == WorldView.Admin ? AdminDisabledItems :AgentDisabledItems ;
		foreach(var currentObject in objectsToDisable)
		{
			currentObject.enabled = false;
		}
	}
}
