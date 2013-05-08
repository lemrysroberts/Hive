using System.Collections;
using UnityEngine;

public class DoorObject : LevelObject
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
	
}
