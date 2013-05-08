using UnityEngine;
using System.Collections;

public abstract class SensorTarget : MonoBehaviour
{
	public abstract void SensorActivate();
	public abstract void SensorDeactivate();
}
