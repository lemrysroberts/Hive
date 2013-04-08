using UnityEngine;
using System.Collections;

public class AnimatedTileMesh : MonoBehaviour 
{
	public void Start()
	{
		if(SpriteDataPath != null && SpriteDataPath != string.Empty)
		{
			m_spriteData = new SpriteData();
			m_spriteData.Load(SpriteDataPath);
		}
	}
	
	void FixedUpdate() 
	{
		timeProgress += Time.deltaTime;
		
		if(timeProgress > 1.0f)
		{
			timeProgress = 0.0f;
		
			if(m_spriteData != null)
			{
				MeshRenderer renderer = GetComponent<MeshRenderer>();
				
				Vector4 offset = m_spriteData.CurrentAnimation.Advance();
				renderer.material.SetTextureOffset("_MainTex", new Vector2(offset.x, offset.y));
				renderer.material.SetTextureScale("_MainTex", new Vector2(offset.z, offset.w));
			}
		}
	}
	
	private float timeProgress = 0;
	
	public string SpriteDataPath 	= null;
	private SpriteData m_spriteData = null;
}
