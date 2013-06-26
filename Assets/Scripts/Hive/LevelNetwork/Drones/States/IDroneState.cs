///////////////////////////////////////////////////////////
// 
// IDroneState.cs
//
// What it does: Interface for drone AI states.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public interface IDroneState
{
	void Start();
	UpdateResult Update();
}

public enum UpdateResult
{
	Updating,
	Complete,
	Failed
}
