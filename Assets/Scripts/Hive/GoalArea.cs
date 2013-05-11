using UnityEngine;
using System.Collections;

public class GoalArea : MonoBehaviour {

	public void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<GoalItem>() != null)
		{
			Debug.Log("Game won");	
			GameFlow.Instance.GameWon();
		}
	}
}
