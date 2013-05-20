/// <summary>
/// 
/// Level object.
/// 
/// This is a base class that contains information about the placement and parameters of an
/// object within the level. This is needed to create view-agnostic definitions that either the 
/// admin-view or agent-view can use to create their final level layouts.
/// 
/// </summary>

using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class LevelObject
{
	public LevelObject()
	{
		m_ID = MaxID;
		MaxID++;
	}
	
	public int ID { get { return m_ID; } }
	
	/// <summary>
	/// Sets the instantiated-prefab's NetworkView to use the given ID, if it exists.
	/// </summary>
	public void SetNetworkViewID(NetworkViewID id)
	{
		if(m_instantiatedPrefab != null)
		{
			NetworkView view = m_instantiatedPrefab.GetComponent<NetworkView>();
			if(view != null)
			{
				view.viewID = id;
			}
			else
			{
				Debug.Log("Instantiated prefab does not have a NetworkView. No synchronisation will occur.");		
			}
		}
	}
	
	public bool GetNetworkViewID(out NetworkViewID viewID)
	{
		if(m_instantiatedPrefab != null)
		{
			NetworkView view = m_instantiatedPrefab.GetComponent<NetworkView>();
			if(view != null)
			{
				view.viewID = Network.AllocateViewID();
				viewID = view.viewID;
				return true;
			}
			
		}
		viewID = new NetworkViewID();
		return false;
		
	}
	
	public GameObject InstantiateAgent()
	{
		GameObject newObject = null;
		if(AgentPrefab != null)
		{
			newObject = GameObject.Instantiate(AgentPrefab) as GameObject;
			newObject.transform.position = Position;
			newObject.transform.localRotation = Rotation;
		}		
		m_instantiatedPrefab = newObject;
		
		LinkNetworkViewToSyncScript();
		
		return newObject;
	}
	
	public GameObject InstantiateAdmin()
	{
		GameObject newObject = null;
		if(AdminPrefab != null)
		{
			newObject = GameObject.Instantiate(AdminPrefab) as GameObject;
			newObject.transform.position = Position;
			newObject.transform.localRotation = Rotation;
		}
		m_instantiatedPrefab = newObject;
		
		LinkNetworkViewToSyncScript();
		
		return newObject;
	}
	
	/// <summary>
	/// A complete mouthful.
	/// This just looks for the specified synchronisation-script in the instantiated prefab and sets it to be the target
	/// of the prefab's NetworkView if it has one.
	/// </summary>
	private void LinkNetworkViewToSyncScript()
	{
		if(SynchronisationScript != null && m_instantiatedPrefab != null)
		{
			MonoBehaviour syncScript = m_instantiatedPrefab.GetComponent(SynchronisationScript) as MonoBehaviour;
			if(syncScript != null)
			{
				// Now that a script is found for synchronisation, check there's a NetworkView and tie them together.
				NetworkView networkView = m_instantiatedPrefab.GetComponent<NetworkView>();
				if(networkView != null)
				{
					networkView.observed = syncScript;
				}
				else
				{
					Debug.LogError("Prefab for LevelObject \"" + this.GetType().ToString() + "\" does not contain a NetworkView component. Admin/Agent objects will not synchronise");
				}
			}
			else
			{
				Debug.LogError("Prefab for LevelObject \"" + this.GetType().ToString() + "\" does not contain script \"" + SynchronisationScript.ToString() + "\". Admin/Agent objects will not synchronise");	
			}
		}
		else
		{
			Debug.LogError("LevelObject \"" + this.GetType().ToString() + "\" has no SynchronisationScript. Admin/Agent objects will not synchronise");	
		}
	}
	
	public GameObject AgentPrefab;
	public GameObject AdminPrefab;
	public Type SynchronisationScript;
	public Vector3 Position;	
	public Quaternion Rotation;
	
	public static int MaxID = 0;
	
	protected int m_ID = -1;
	
	protected GameObject m_instantiatedPrefab = null;
}
 