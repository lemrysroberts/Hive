/// <summary>
/// 
/// Level object.
/// 
/// This is a base class that contains information about the placement and parameters of an
/// object within the level. This is needed to create view-agnostic definitions that either the 
/// admin-view or agent-view can use to create their final level layouts.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public abstract class LevelObject 
{
	public abstract GameObject InstantiateAdmin();
	public abstract GameObject InstantiateAgent();
	
	public GameObject Prefab;
	public Vector3 Position;
	public Quaternion Rotation;
}
 