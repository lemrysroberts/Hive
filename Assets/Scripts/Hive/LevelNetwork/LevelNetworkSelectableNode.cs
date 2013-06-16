using UnityEngine;
using System.Collections;

public class LevelNetworkSelectableNode : MonoBehaviour 
{
	public GameObject ClaimBarObject = null;
	public GameObject ClaimBarBackingObject = null;
	public LevelNetworkNode m_node = null;
	public float ClaimBarWidth = 3.0f;
	
	// Use this for initialization
	void Start () 
	{
		if(ClaimBarBackingObject != null)
		{
			Vector3 scale = ClaimBarBackingObject.transform.localScale;
			ClaimBarBackingObject.transform.localScale = new Vector3(ClaimBarWidth, scale.y, scale.z);
		}
		
		if(m_node.NodeTexture != null)
		{
			renderer.material.mainTexture = m_node.NodeTexture;
			renderer.material.SetColor("_Color", Color.white);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(m_node.ActivityInProgress && ClaimBarObject != null && ClaimBarBackingObject != null)
		{
			if(!ClaimBarObject.activeSelf)
			{
				ClaimBarObject.SetActive(true);
				ClaimBarBackingObject.SetActive(true);
			}
			
			ClaimBarObject.transform.localScale = new Vector3(m_node.ActivityProgress * ClaimBarWidth, ClaimBarObject.transform.localScale.y, ClaimBarObject.transform.localScale.z);
		}
		else if(ClaimBarObject != null && ClaimBarBackingObject != null)
		{
			if(ClaimBarObject.activeSelf)
			{
				ClaimBarObject.SetActive(false);	
				ClaimBarBackingObject.SetActive(false);
			}
		}
	}
}
