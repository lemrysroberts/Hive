using UnityEngine;
using System.Collections;

public abstract class SensorTarget : SaveSerialisable
{
	public abstract void SensorActivate();
	public abstract void SensorDeactivate();
}
