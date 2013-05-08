/// <summary>
/// Goal item object.
/// 
/// Winner of the worst-named class ever, mayhap?
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GoalItemObject : LevelObject
{
	public override GameObject InstantiateAgent()
	{
		GameObject newObject = null;
		if(Prefab != null)
		{
			newObject = GameObject.Instantiate(Prefab) as GameObject;
			newObject.transform.position = Position;
			newObject.transform.localRotation = Rotation;
		}
		
		return newObject;
	}
	
	public override GameObject InstantiateAdmin()
	{
		GameObject newObject = null;
		if(Prefab != null)
		{
			newObject = GameObject.Instantiate(Prefab) as GameObject;
			newObject.transform.position = Position;
			newObject.transform.localRotation = Rotation;
			
			GameObject panelObject = null;
			for(int childID = 0; childID < newObject.transform.childCount && panelObject == null; childID++)
			{
				if(newObject.transform.GetChild(childID).name == "Panel")
				{
					panelObject = newObject.transform.GetChild(childID).gameObject;	
				}
			}
			/*
			if(panelObject != null)
			{
				MeshRenderer renderer = panelObject.GetComponent<MeshRenderer>();
				
				if(m_cachedAdminMaterial == null)
				{
					m_cachedAdminMaterial =  AssetHelper.Instance.GetAsset<Material>("Materials/door_panel_alt") as Material;
				}
				
				renderer.material = m_cachedAdminMaterial;
			}
			*/
		}
		
		return newObject;
	}
}
