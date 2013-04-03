using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;

public class AssetHelper 
{

	
	public UnityEngine.Object GetAsset<T>(string assetPath)
	{
#if UNITY_EDITOR
		UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
#else
		UnityEngine.Object asset = Resources.Load(assetPath);
#endif
		
		return asset;
	}
	
	public static AssetHelper Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new AssetHelper();	
			}
			
			return s_instance;
		}
	}
	
	private AssetHelper() {}
	
	private static AssetHelper s_instance = null;
}
