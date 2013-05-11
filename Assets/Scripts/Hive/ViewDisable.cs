using UnityEngine;
using System.Collections.Generic;

public class ViewDisable : MonoBehaviour 
{
	public List<MonoBehaviour> AdminDisabledItems = new List<MonoBehaviour>();
	public List<MonoBehaviour> AgentDisabledItems = new List<MonoBehaviour>();
	
	// Use this for initialization
	void Start () 
	{
		WorldView view = GameFlow.Instance.View;
		
		List<MonoBehaviour> objectsToDisable = view == WorldView.Admin ? AdminDisabledItems :AgentDisabledItems ;
		foreach(var currentObject in objectsToDisable)
		{
			currentObject.enabled = false;
		}
	}
}
