///////////////////////////////////////////////////////////
// 
// InteractiveObjectNotifier.cs
//
// What it does: Provides a link between an object's interaction trigger and controlling script.
//				 For flexibility, an object may want both a physics collider as well as an interaction-trigger.
//				 In order to achieve this, a child gameObject is needed. This component can be added to form a link
//				 back to the level-object.
//
// Notes: Not architecturally great, but relatively light.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class InteractiveObjectNotifier : MonoBehaviour 
{
	public InteractiveObject TargetObject
	{
		get { return m_targetObject; }	
	}
	
	// Use this for initialization
	void Start () 
	{
		// Search up the hierarchy for an InteractiveObject.
		Transform current = transform;
		
		while(current != null && m_targetObject == null)
		{
			InteractiveObject[] components = current.gameObject.GetComponents<InteractiveObject>();
			
			foreach(var component in components)
			{
				if(component is InteractiveObject)
				{
					m_targetObject = component as InteractiveObject;	
				}	
			}
			
			current = current.parent;
		}
		
		if(m_targetObject == null)
		{
			Debug.LogWarning("target-object not set. Colliders will not be able to find the parent object");
		}
	}
	
	private InteractiveObject m_targetObject = null;
}
