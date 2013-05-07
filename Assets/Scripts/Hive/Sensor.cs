using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour 
{
	public SensorTarget Target = null;
		
	void OnTriggerEnter(Collider other)
	{
		if(Target != null && other.tag == "Player" || other.tag == "Entity")
		{
			Target.SensorActivate();
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(Target != null && other.tag == "Player" || other.tag == "Entity")
		{
			Target.SensorDeactivate();
		}
	}
}
