using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DON'T LOOK AT ME, I'M HIDEOUS.
/// 
/// This thing is Dr Wright's fetish material. 
/// 
/// The following list details all the idiotic quirks of this class.
/// 
/// 
/// 
/// </summary>

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SphereCollider))]
public class AgentCamera : MonoBehaviour 
{
	[SerializeField]
	public LayerMask collisionLayer = 0;
	
	public bool ShowCandidateRays = true;
	public bool ShowSucceededRays = true;
	public bool ShowExtrusionRays = true;
	
	public float m_nudgeMagnitude 	= 0.15f; // This determines how far vertices are extruded from their mesh centroid when casting rays. Needed to prevent false collisions.
	private MeshFilter m_filter 	= null;
	private Mesh m_cameraMesh;
	private SphereCollider m_viewCollider = null;
	private Dictionary<Collider, BoxColliderVertices> m_colliderVertices = new Dictionary<Collider, BoxColliderVertices>();
	
	void Start () 
	{
		m_filter 		= GetComponent<MeshFilter>();
		m_viewCollider 	= GetComponent<SphereCollider>();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("LevelGeo") && other is BoxCollider)
		{
			BoxCollider collider = other as BoxCollider;
			BoxColliderVertices newVertices = new BoxColliderVertices();
			
			newVertices.collider = collider;
			
			// I have a sneaky suspicion these divided values already exist in the collider-data.
			newVertices.vertices[0] = new Vector3(-collider.size.x / 2.0f, -collider.size.y / 2.0f, 0.0f);
			newVertices.vertices[1] = new Vector3(-collider.size.x / 2.0f, collider.size.y / 2.0f, 0.0f);
			newVertices.vertices[2] = new Vector3( collider.size.x / 2.0f, -collider.size.y / 2.0f, 0.0f);
			newVertices.vertices[3] = new Vector3( collider.size.x / 2.0f, collider.size.y / 2.0f, 0.0f);
			
			for(int vert = 0; vert < 4; vert++)
			{
				newVertices.vertices[vert] = collider.transform.TransformPoint(newVertices.vertices[vert]);	
			//	Debug.Log("Vert: " + newVertices.vertices[vert].x + ", " + newVertices.vertices[vert].y);
			}
			
			m_colliderVertices.Add(other, newVertices);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		m_colliderVertices.Remove(other);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		RebuildMesh();
	}
	
	/// <summary>
	/// Rebuilds the view-mesh from a sorted list of occluders.
	/// </summary>
	private void RebuildMesh()
	{
		List<OccluderVector> occluders = GetOccluders();
		return;
		int triangleCount = ((occluders.Count) - 1) * 3;
		
		Vector3[] 	vertices 	= new Vector3[occluders.Count + 1];
		Vector3[] 	normals 	= new Vector3[occluders.Count + 1];
		Vector2[] 	uvs 		= new Vector2[occluders.Count + 1];
		int[] 		triangles 	= new int[triangleCount];
		
		// Add the camera point manually
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		
		int index = 1;
		
		Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
		
		foreach(OccluderVector vert in occluders)
		{
			Vector3 worldPosition = vert.vec - transform.position;
			vertices[index] = rotationInverse *  worldPosition;
			uvs[index] 		= new Vector2(worldPosition.x, worldPosition.y);
			normals[index] = new Vector3(0.0f, 0.0f -1.0f);
			
			index++;
		}
		
		// Loop through the points and build them there triangles
		for(int i = 1; i < occluders.Count; i++)
		{
			triangles[(i - 1) * 3] = 0;
			triangles[(i - 1) * 3 + 1] = i;
			triangles[(i - 1) * 3 + 2] = i + 1;
		}
		
		m_filter.sharedMesh.Clear();
		m_filter.sharedMesh.vertices 	= vertices;
		m_filter.sharedMesh.uv 			= uvs;
		m_filter.sharedMesh.normals = normals;
		m_filter.sharedMesh.triangles 	= triangles;
	}
	
	/// <summary>
	/// Gets a list of all vertices that occlude the view area.
	/// The OccluderVector structure contains the hit-points of each collision, as well as the angle of the hit-point from the left extent.
	/// </summary>
	/// <returns>
	/// The occluder list, sorted around increasing angle from the left extent.
	/// </returns>
	private List<OccluderVector> GetOccluders()
	{
		Vector3 cameraDirection = Vector3.up;
		float viewDistance = m_viewCollider.radius;
		
		List<VectorOffsetPair> verts = new List<VectorOffsetPair>();
		
		// Loop through each collider and add its vertices to the list
		foreach(var colliderPair in m_colliderVertices)
		{
			/*
			for(int vert = 0; vert < 4; vert++)
			{
				colliderPair.Value.vertices[vert] = collider.transform.TransformPoint(colliderPair.Value.vertices[vert]);	
			//	Debug.Log("Vert: " + newVertices.vertices[vert].x + ", " + newVertices.vertices[vert].y);
			}*/
			
			foreach(Vector3 vert in colliderPair.Value.vertices)
			{
				Vector3 worldPos = collider.transform.TransformPoint(vert);
				Vector3 cameraToVertex = vert - transform.position;
				
				if(ShowCandidateRays)	Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -4.0f), cameraToVertex , Color.magenta);
				
				// This works out the magnitude of the vector if it were to reach the max range.
				// TODO: This could be removed if the camera area was a correct sweep rather than a triangle. The magnitude of the vector would just be
				//		 tested against the range. The difficulty is the lateral sweep becomes harder over a curve.
				float cameraDirVertexDot = Vector3.Dot(cameraDirection, Vector3.Normalize(cameraToVertex));
				float angle = Mathf.Acos(cameraDirVertexDot);
				float mag = (cameraToVertex).magnitude;
				
				float thing = mag * cameraDirVertexDot;
				
				if(thing < viewDistance)
				{
					VectorOffsetPair newPair = new VectorOffsetPair();
					newPair.vec = vert;
				
					Vector3 offsetDirection = newPair.vec - (colliderPair.Value.collider.transform.position);
					newPair.offsetVec = newPair.vec + (offsetDirection * m_nudgeMagnitude);
					newPair.offsetVec.z = newPair.vec.z;
					
					if(ShowExtrusionRays)
					{
						Debug.DrawRay (newPair.vec + new Vector3(0.0f, 0.0f, -4.0f), offsetDirection , Color.red);	
					}
				
					verts.Add(newPair);	
				}
			}
		}
	
		List<Vector3> validVerts = new List<Vector3>();
		validVerts.Clear();
		
		RaycastHit hitInfo;
		
		
		List<OccluderVector> occluders = new List<OccluderVector>();
		
		
		// Iterate through the initial verts and add both valid verts and projected offset vert intersections
		foreach(VectorOffsetPair vert in verts)
		{
			Vector3 directionToVert = vert.vec - this.transform.position;
			
			directionToVert = vert.offsetVec - this.transform.position;
			float magnitude = directionToVert.magnitude * 0.98f;
			
			// Check to see if the original vertex is occluded
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, magnitude, collisionLayer)) 
			{
				// Not occluded. The vert itself is fine. Next we check the projected ray...
				validVerts.Add(vert.vec);
				
				float cosTheta = Vector3.Dot(cameraDirection, Vector3.Normalize(directionToVert));
				float maxMagnitude = viewDistance / cosTheta;
				
				Vector3 maxPosition = transform.position + (Vector3.Normalize(directionToVert) * maxMagnitude);
				Vector3 newDirection = Vector3.Normalize(directionToVert) * maxMagnitude ;
				
				if(!Physics.Raycast(this.transform.position, newDirection, out hitInfo, newDirection.magnitude, collisionLayer))
				{
					validVerts.Add(maxPosition);
				}
				else
				{
					validVerts.Add (hitInfo.point);	
				}
			}
		}
		
		// Output all results
		foreach(Vector3 vert in validVerts)
		{
			Vector3 directionToVert = vert - this.transform.position;
			
			OccluderVector newOccluder = new OccluderVector();
			newOccluder.vec = vert;
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			float angle = 0;//Mathf.Acos(Vector3.Dot(Vector3.Normalize(leftDirection), normalDirection));
			
			
			newOccluder.angle = angle;
			occluders.Add(newOccluder);
		}
	
		occluders.Sort(OccluderComparison);
		
		if(ShowSucceededRays)
		{
			foreach(OccluderVector vert in occluders)
			{
				Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), vert.vec - this.transform.position , Color.green);
			}
		}
		
		return occluders;
	}
			
	private static int OccluderComparison(OccluderVector v1, OccluderVector v2)
	{
		if(v1.angle == v2.angle)
		{
			// If the angles are equivalent, sort on their distance from the camera
			if( v1.vec.sqrMagnitude > v2.vec.sqrMagnitude)
			{
				return 1;	
			}
			else if( v2.vec.sqrMagnitude < v1.vec.sqrMagnitude)
			{
				return -1;	
			}
			return 0;
		}
		
		if(v1.angle > v2.angle)
		{
			return 1;	
		}
		
		return -1;
	}
	
	private class VectorOffsetPair
	{
		public Vector3 vec;
		public Vector3 offsetVec;
	}
	
	private class OccluderVector
	{
		public Vector3 vec;
		public float angle;
	}
	
	private class BoxColliderVertices
	{
		public BoxCollider collider;
		public Vector3[] vertices = new Vector3[4];	
	}
		
}
