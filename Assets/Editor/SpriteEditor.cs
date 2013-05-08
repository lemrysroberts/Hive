using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Sprite))] 
public class SpriteEditor : Editor
{
	
	public override void OnInspectorGUI()
	{
		Sprite sprite = (Sprite)target;
		if(GUILayout.Button(sprite.SpriteData))
		{
			sprite.SpriteData = EditorUtility.OpenFilePanel("Open Sprite Data", "", "xml");
			sprite.SpriteData = AssetHelper.StripResourcePath(sprite.SpriteData);
		}
		
		sprite.SpriteTexture = EditorGUILayout.ObjectField(sprite.SpriteTexture, typeof(Texture2D), false) as Texture2D;
		sprite.updateSpeed = EditorGUILayout.FloatField("Update Speed", sprite.updateSpeed);
	}
	
}
