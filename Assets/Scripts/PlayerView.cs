using UnityEngine;
using System;
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
public class PlayerView : MonoBehaviour 
{
	[SerializeField]
	public LayerMask collisionLayer = 0;
	
	public bool ShowCandidateRays 	= false;
	public bool ShowSucceededRays 	= true;
	public bool ShowExtrusionRays 	= false;
	public bool ShowFailedRays		= false;
	
	public float m_nudgeMagnitude 	= 0.05f; // This determines how far vertices are extruded from their mesh centroid when casting rays. Needed to prevent false collisions.
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
			BoxCollider newCollider = other as BoxCollider;
			BoxColliderVertices newVertices = new BoxColliderVertices();
			
			newVertices.collider = newCollider;
			
			// I have a sneaky suspicion these divided values already exist in the collider-data.
			newVertices.vertices[0] = new Vector3(-newCollider.size.x / 2.0f, -newCollider.size.y / 2.0f, 0.0f);
			newVertices.vertices[1] = new Vector3(-newCollider.size.x / 2.0f, newCollider.size.y / 2.0f, 0.0f);
			newVertices.vertices[2] = new Vector3( newCollider.size.x / 2.0f, -newCollider.size.y / 2.0f, 0.0f);
			newVertices.vertices[3] = new Vector3( newCollider.size.x / 2.0f, newCollider.size.y / 2.0f, 0.0f);
			
			// TODO: 	This is a bodge for a relatively complex problem. Take it out and observe the carnage.
			// 			Now, this seems to work at the moment, but I'm sure there are mathematical edge cases that will trip it up.
			newVertices.vertices[4] = new Vector3( 0.0f, 0.0f, 0.0f);
			
			for(int vert = 0; vert < newVertices.vertices.Length; vert++)
			{
				//newVertices.vertices[vert] = newCollider.transform.TransformPoint(newVertices.vertices[vert]);	
			}
			
			m_colliderVertices.Add(other, newVertices);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		m_colliderVertices.Remove(other);
	}
	
	// Update is called once per frame
	void Update () 
	{
		RebuildMesh();
	}
	
	/// <summary>
	/// Rebuilds the view-mesh from a sorted list of occluders.
	/// </summary>
	private void RebuildMesh()
	{
		List<OccluderVector> occluders = GetOccluders();
		
		if(occluders.Count == 0)
		{
			m_filter.sharedMesh.Clear();
			return;	
		}
		//return;
		int triangleCount = ((occluders.Count) - 1) * 3;
		
		Vector3[] 	vertices 	= new Vector3[occluders.Count + 1];
		Vector3[] 	normals 	= new Vector3[occluders.Count + 1];
		Vector2[] 	uvs 		= new Vector2[occluders.Count + 1];
		int[] 		triangles 	= new int[triangleCount + 3];
		
		// Add the camera point manually
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		uvs[0] = new Vector2(0.5f, 0.5f);
		
		int index = 1;
		
		Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
		
		float extentsVal = Mathf.Abs(m_viewCollider.radius * Mathf.Cos(Mathf.PI / 4));
		
		foreach(OccluderVector vert in occluders)
		{
			Vector3 localPosition = vert.vec - transform.position;
			vertices[index] = rotationInverse *  localPosition;
			uvs[index] 		= new Vector2((localPosition.x + extentsVal) / (extentsVal * 2.0f), (localPosition.y + extentsVal) / (extentsVal * 2.0f));
			normals[index] = new Vector3(0.0f, 0.0f -1.0f);
			
			index++;
		}
		
		// Loop through the points and build them there triangles
		int i = 1;
		for(; i < occluders.Count; i++)
		{
			triangles[(i - 1) * 3] = 0;
			triangles[(i - 1) * 3 + 1] = i;
			triangles[(i - 1) * 3 + 2] = i + 1;
		}
		
		triangles[(i - 1) * 3] = 0;
		triangles[(i - 1) * 3 + 1] = i;
		triangles[(i - 1) * 3 + 2] = 1;
		
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
		List<VectorOffsetPair> verts = new List<VectorOffsetPair>();
		
		// Loop through each collider and add its vertices to the list
		foreach(var colliderPair in m_colliderVertices)
		{
			foreach(Vector3 vert in colliderPair.Value.vertices)
			{
				Vector3 worldPos = colliderPair.Key.transform.TransformPoint(vert);
				Vector3 cameraToVertex = worldPos - transform.position;
				
				if(ShowCandidateRays)	Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -4.0f), cameraToVertex , Color.magenta);
		
				
				//if(thing < viewDistance)
				{
					VectorOffsetPair newPair = new VectorOffsetPair();
					newPair.vec = worldPos;
					
					if(vert != Vector3.zero)
					{
				
						Vector3 offsetDirection = newPair.vec - (colliderPair.Value.collider.transform.position);
						newPair.offsetVec = newPair.vec + (offsetDirection * m_nudgeMagnitude);
						newPair.offsetVec.z = newPair.vec.z;
						
						if(ShowExtrusionRays)
						{
							Debug.DrawRay (newPair.vec + new Vector3(0.0f, 0.0f, -4.0f), offsetDirection , Color.red);	
						}
					}
				
					verts.Add(newPair);	
				}
			}
		}
		
		List<VectorOffsetPair> extentsPairs = GetExtents();
		
		foreach(var pair in extentsPairs)
		{
			verts.Add(pair);	
		}
	
		List<Vector3> validVerts = new List<Vector3>();
		validVerts.Clear();
		
		RaycastHit hitInfo;
		
		List<OccluderVector> occluders = new List<OccluderVector>();
		
		// Iterate through the initial verts and add both valid verts and projected offset vert intersections
		foreach(VectorOffsetPair vert in verts)
		{
			Vector3 directionToVert = vert.vec - this.transform.position;
			float magnitude = directionToVert.magnitude * 0.98f;
			
			// Check to see if the original vertex is occluded
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, magnitude, ~collisionLayer)) 
			{
				// Not occluded. The vert itself is fine. Next we check the projected ray...
				validVerts.Add(vert.vec);
				
				if(vert.offsetVec != Vector3.zero)
				{
					directionToVert = vert.offsetVec - this.transform.position;
					directionToVert.z = 0.0f;
				
					float maxMagnitude = m_viewCollider.radius;//viewDistance / cosTheta;
					
					
					Vector3 newDirection = Vector3.Normalize(directionToVert) * maxMagnitude ;
					Vector3 maxPosition = transform.position + (newDirection);
					if(!Physics.Raycast(this.transform.position, newDirection, out hitInfo, newDirection.magnitude, ~collisionLayer))
					{
						
						Debug.DrawRay(this.transform.position + new Vector3(0.0f, 0.0f, -2.0f), newDirection  + new Vector3(0.0f, 0.0f, -2.0f), Color.yellow);
						validVerts.Add(maxPosition);
					}
					else
					{
						validVerts.Add (hitInfo.point);	
					}	
				}
			}
			else 
			{
				validVerts.Add(hitInfo.point);
				
				if(ShowFailedRays)
				{
					Vector3 hitStart = hitInfo.point - new Vector3(0.1f, 0.0f, 0.0f);
					Vector3 hitEnd = hitInfo.point + new Vector3(0.1f, 0.0f, 0.0f);
					Debug.DrawLine(hitStart, hitEnd, Color.blue);
				
					Debug.DrawRay(this.transform.position, directionToVert * 0.9f , Color.red);	
				}
			}
		}
		
		// Output all results
		foreach(Vector3 vert in validVerts)
		{
			Vector3 directionToVert = vert - this.transform.position;
			
			
			
			OccluderVector newOccluder = new OccluderVector();
			newOccluder.vec = vert;
			newOccluder.vec.z = transform.position.z;
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			float dot = Vector3.Dot(Vector3.up, normalDirection);
			
			double angle = Math.Atan2((double)normalDirection.x, (double)normalDirection.y);
			if(double.IsNaN(angle))
			{
				Debug.LogWarning("NaN found in PlayerView: " + dot);	
			}
			
			newOccluder.angle = angle;
			occluders.Add(newOccluder);
		}
	
		occluders.Sort(OccluderComparison);
		
		if(ShowSucceededRays)
		{
			Color color1 = new Color(1.0f, 0.5f, 0.0f, 1.0f);
			Color color2 = new Color(0.5f, 1.0f, 0.0f, 1.0f);
			bool useColor1 = false;
			
			foreach(OccluderVector vert in occluders)
			{
				useColor1 = !useColor1;
				Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), vert.vec - this.transform.position , useColor1 ? color1 : color2);
			}
		}
		
		return occluders;
	}
	
	private List<VectorOffsetPair> GetExtents()
	{
		List<VectorOffsetPair> verts = new List<VectorOffsetPair>();
		
		VectorOffsetPair tlPair = new VectorOffsetPair();
		VectorOffsetPair blPair = new VectorOffsetPair();
		VectorOffsetPair trPair = new VectorOffsetPair();
		VectorOffsetPair brPair = new VectorOffsetPair();
		
		float extentsVal = Mathf.Abs(m_viewCollider.radius * Mathf.Cos(Mathf.PI / 4));
		
		tlPair.vec = transform.position + new Vector3(-extentsVal, extentsVal, 0.0f);
		blPair.vec = transform.position + new Vector3(-extentsVal, -extentsVal, 0.0f);
		trPair.vec = transform.position + new Vector3(extentsVal, extentsVal, 0.0f);
		brPair.vec = transform.position + new Vector3(extentsVal, -extentsVal, 0.0f);
		
		tlPair.offsetVec = Vector3.zero;
		blPair.offsetVec = Vector3.zero;
		trPair.offsetVec = Vector3.zero;
		brPair.offsetVec = Vector3.zero;
		
		verts.Add(tlPair);
		verts.Add(blPair);
		verts.Add(trPair);
		verts.Add(brPair);
	
		return verts;
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
		public Vector3 offsetVec = Vector3.zero;
	}
	
	private class OccluderVector
	{
		public Vector3 vec;
		public double angle;
	}
	
	private class BoxColliderVertices
	{
		public BoxCollider collider;
		public Vector3[] vertices = new Vector3[5];	
	}
		
}
