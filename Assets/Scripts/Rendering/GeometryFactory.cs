using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (MeshFilter))]
public class GeometryFactory : MonoBehaviour 
{
	public enum GeometryType
	{
		Plane,	
		Triangle
	}
	
	public GeometryType geometryType = GeometryType.Plane;
	public bool ScaleUVs = false;
	public float UVScale = 1.0f;
	
	[SerializeField]
	private bool m_meshBuilt = false;
	
	// Use this for initialization
	void Start () 
	{
		if(!m_meshBuilt)
		{
			RebuildMesh();	
		}
	}
	
	public void RebuildMesh()
	{
		MeshFilter mesh = GetComponent<MeshFilter>();
			
		if(mesh != null)
		{
			if(ScaleUVs)
			{
				mesh.sharedMesh = CreatePlane(transform.localScale.x * UVScale, transform.localScale.y * UVScale);		
			}
			else
			{
				mesh.sharedMesh = CreatePlane(1.0f, 1.0f);		
			}
		}
		m_meshBuilt = true;
	}
	
	public static Mesh CreatePlane(float UVXScale, float UVYScale)
	{
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:Plane";
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector2[] 	uvs 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-0.5f, -0.5f, 0.0f);
		vertices[1] = new Vector3(0.5f, -0.5f, 0.0f);
		vertices[2] = new Vector3(-0.5f, 0.5f, 0.0f);
		vertices[3] = new Vector3(0.5f, 0.5f, 0.0f);
		
		uvs[0] = new Vector2(0.0f, 0.0f);
		uvs[1] = new Vector2(UVXScale, 0.0f);
		uvs[2] = new Vector2(0.0f, UVYScale);
		uvs[3] = new Vector2(UVXScale, UVYScale);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices = vertices;
		newMesh.uv = uvs;
		newMesh.triangles = triangles;
		
		return newMesh;
	}
	
	
}
