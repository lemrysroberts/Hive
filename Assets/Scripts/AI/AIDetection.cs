using UnityEngine;
using System.Collections;

public interface IDetectionListener
{
	void DetectionMade(Vector3 detectionPoint);
	void DetectionLost();
}

public class AIDetection : MonoBehaviour {
	
	public GameObject Target;
	public MonoBehaviour NotificationScript;
	public float MaxRange = 10; //Range in world units
	public float FOV = 45;// Field of view in degrees, based on the Gameobjects "left" transform
	public bool DrawDebug = false;
	private Vector3 PreviousPosition;
	
	// Use this for initialization
	void Start () 
	{
		if(NotificationScript != null)
		{
			// Sorry for this weirdness. Unity's editor doesn't support C# interfaces, so I can't ensure 
			// that the passed MonoBehaviour implements IDetectionListener without making a distinct editor class.
			if(!(NotificationScript is IDetectionListener))
			{
				Debug.LogError("NotificationScript does not implement IDetectedNotifier. Detections will not be registered");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {		
		if(Target != null) {
			float maxRangeSquared = MaxRange * MaxRange;
			Vector3 targetDirection = Target.transform.position - transform.position;
			//Might need to change this depending on how we interpret what the "forward" direction is for the AI
			Vector3 myDirection = PreviousPosition - transform.position;
			float angleToTarget = Vector3.Dot(Vector3.Normalize(targetDirection), Vector3.Normalize(myDirection));
			
			if(DrawDebug){
				Debug.DrawLine(transform.position, transform.position + Vector3.Normalize(targetDirection) * MaxRange, Color.red);
				Debug.DrawLine(transform.position, transform.position + myDirection, Color.white);
			}
			
			//first part in range, second in viewing cone
			if(targetDirection.sqrMagnitude < maxRangeSquared && angleToTarget > FOV*Mathf.Deg2Rad/2){
				RaycastHit result;
				if(Physics.Raycast(transform.position, targetDirection, out result, MaxRange) 
					&& result.collider.gameObject == Target) {
					//Detected, do stuff or add event or delegate or whatevers
					
					if(NotificationScript != null)
					{
						IDetectionListener notifier = NotificationScript as IDetectionListener;
						if(notifier != null)
						{
							notifier.DetectionMade(transform.position + targetDirection);	
						}
					}
					
					if(DrawDebug){
						Debug.DrawRay(transform.position, targetDirection, Color.blue);
					}
				}
				else{
					//No hit
					if(NotificationScript != null)
					{
						IDetectionListener notifier = NotificationScript as IDetectionListener;
						if(notifier != null)
						{
							notifier.DetectionLost();	
						}
					}
				}
			}
			
			PreviousPosition = transform.position;
		}
	}
}
