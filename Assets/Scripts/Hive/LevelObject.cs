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

[Serializable]
public sealed class LevelObject : ICloneable
{
	public LevelObject(string objectName)
	{
		Name = objectName;
		m_ID = MaxID;
		MaxID++;
	}
	
	public int ID 
	{ 
		get { return m_ID; } 
	}
	
	public string Name
	{
		get { return m_name; }
		set { m_name = value; }
	}
	
	public System.Object Clone()
	{
		LevelObject clone = new LevelObject(Name);
		
		clone.AdminPrefab = AdminPrefab;
		clone.AgentPrefab = AgentPrefab;
		clone.SynchronisationScript = SynchronisationScript;
		clone.Position = Position;
		clone.Rotation = Rotation;
		
		return clone;
	}
	
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
			Type syncScriptType = Type.GetType(SynchronisationScript);
			if(SynchronisationScript == string.Empty || SynchronisationScript == null)
			{
				Debug.LogWarning("No SynchronisationScript set for object \"" + Name + "\".");
				return;
			}
			
			if(syncScriptType == null) 
			{
				Debug.LogError("Failed to find TypeInfo for " + SynchronisationScript);
				return;
			}
			
			MonoBehaviour syncScript = m_instantiatedPrefab.GetComponent(syncScriptType) as MonoBehaviour;
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
					Debug.LogWarning("Prefab for LevelObject \"" + this.GetType().ToString() + "\" does not contain a NetworkView component. Admin/Agent objects will not synchronise");
				}
			}
			else
			{
				Debug.LogWarning("Prefab for LevelObject \"" + this.GetType().ToString() + "\" does not contain script \"" + SynchronisationScript + "\". Admin/Agent objects will not synchronise");	
			}
		}
		else
		{
			Debug.LogWarning("LevelObject \"" + this.GetType().ToString() + "\" has no SynchronisationScript. Admin/Agent objects will not synchronise");	
		}
	}
	
	
	
	[SerializeField]
	public GameObject AgentPrefab;
	
	[SerializeField]
	public GameObject AdminPrefab;
	
	[SerializeField]
	public string SynchronisationScript;
	
	[SerializeField]
	public Vector3 Position;	
	
	[SerializeField]
	public Quaternion Rotation;
	
	[SerializeField]
	public static int MaxID = 0;
	
	[SerializeField]
	private int m_ID = -1;
	
	[SerializeField]
	private GameObject m_instantiatedPrefab = null;
	
	[SerializeField]
	private string m_name = string.Empty;
}
 