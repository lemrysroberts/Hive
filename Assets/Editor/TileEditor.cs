using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileEditor : EditorWindow
{
	[MenuItem("Tiles/Tile Manager")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(TileEditor));
	}
	
	Vector2 scrollPos = Vector2.zero;
	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Save", GUILayout.Width(100)))
		{
			string tileset = EditorUtility.SaveFilePanel("Save Tileset", "Resources", "tiles", "xml");
			TileManager.Instance.Save(tileset);
		}
		
		if(GUILayout.Button("Load", GUILayout.Width(100)))
		{
			string tileset = EditorUtility.OpenFilePanel("Load Tileset", "Resources", "xml");	
			TileManager.TileSetFilename = tileset;
			TileManager.Instance.Init();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		
		
		DrawTileList();
		GUILayout.Box("", GUILayout.Height(position.height - 30), GUILayout.Width(1));
		
		DrawDetails();
		
		GUILayout.EndHorizontal();
	}
	
	private void DrawTileList()
	{
		GUI.depth = 100;
		TileManager manager = TileManager.Instance;
	
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(m_listWidth));
		
		
		foreach(var tilePair in manager.Tiles)
		{
			Tile tile = tilePair.Value;
			
			GUILayout.BeginHorizontal();
			
			tile.SetTexture( EditorGUILayout.ObjectField( tile.GetTexture(), typeof(Texture), false, GUILayout.Width(40), GUILayout.Height(40)) as Texture);
			
			GUILayout.BeginVertical(GUILayout.Width(100));
				
	
			
			if(GUILayout.Button("Select", GUILayout.Width(80)))
			{
				TileManager.Instance.SelectedTile = tile;	
			}
			
			GUILayout.EndHorizontal();
			
			
			GUILayout.EndHorizontal();
			GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(120));
		}
		
		//if(GUI.Button( new Rect(10, y + 10, 100, 20), "+"))
		{
		//	Tile newTile = manager.AddTile();
			//newTile.TextureID = "sdf";
		}
		
		GUILayout.EndScrollView();
		
	}
	
	private void DrawDetails()
	{
		TileManager manager = TileManager.Instance;
		
		Tile tile = manager.SelectedTile;
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		tile.SetTexture( EditorGUILayout.ObjectField( tile.GetTexture(), typeof(Texture), false, GUILayout.Width(100), GUILayout.Height(100)) as Texture);
		EditorGUILayout.LabelField(tile.TextureID);
		
		GUILayout.EndHorizontal();
		
		GUILayout.Box("", GUILayout.Width(position.width - m_listWidth - 10), GUILayout.Height(1));
		
		tile.NavBlock = GUILayout.Toggle(tile.NavBlock, "Nav-Block");
		tile.Animated = GUILayout.Toggle(tile.Animated, "Animated");
		
		if(GUILayout.Button(tile.SpriteDataPath == null ? "No Sprite Data" : tile.SpriteDataPath))
		{
			string spriteData = EditorUtility.OpenFilePanel("Sprite Data", "Sprite Data", "xml");
			spriteData = AssetHelper.StripResourcePath(spriteData);
				
			tile.SpriteDataPath = spriteData;
		}
		
		GUILayout.EndVertical();
	}
	
	private const int m_listWidth = 160;
}
