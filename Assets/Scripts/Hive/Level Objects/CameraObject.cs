using UnityEngine;
using System.Collections;

public class CameraObject : LevelObject
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
		}
		
		return newObject;
	}
}
