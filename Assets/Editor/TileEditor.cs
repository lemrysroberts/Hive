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
		int y = 0;
		
		TileManager manager = TileManager.Instance;
		
		List<int> toDelete = new List<int>();
		
		
		
		int maxHeight = manager.Tiles.Count * 71; // 71 is the per-item height
		maxHeight += 60;
		 
		// Menu
		if(GUI.Button(new Rect(5, 3, 50, 20), "Save"))
		{
			string levelFile = EditorUtility.SaveFilePanel("Save Tiles", "Tiles", "tileset", "xml");
			TileManager.Instance.Save(levelFile);
		}
		
		GUI.Box(new Rect(5, 25, position.width - 10, 1),"");
		
        // Set up a scroll view
        scrollPos = GUI.BeginScrollView (new Rect (0, 30, position.width, position.height), scrollPos, new Rect (0, 0, 1000, maxHeight));
		
		y = 5;
		GUI.Label(new Rect(10, y, 100, 20), "Tiles");
		
		y += 20; // Label height
		
		foreach(var tilePair in manager.Tiles)
		{
			Tile tile = tilePair.Value;
			
			if(tile == manager.SelectedTile)
			{
				GUI.Box(new Rect(0.0f, y, position.width, 71.0f), "");	
			}
		
			y += 5;
			
			GUI.Box(new Rect(5, y, position.width - 10, 1),"");
			
			y += 6;
	
			if(GUI.Button(new Rect(70, y + 25, 80, 20), "Delete"))
			{ 
				toDelete.Add(tile.ID);
			}
			
			if(GUI.Button(new Rect(70, y, 80, 20), "Select"))
			{
				TileManager.Instance.SelectedTile = tile;	
			}
			
			tile.SetTexture( EditorGUI.ObjectField(new Rect(10, y , 50, 50), tile.GetTexture(), typeof(Texture)) as Texture);
			y += 50;
			
		}
		
		foreach(int toDeleteID in toDelete)
		{
			manager.RemoveTile(toDeleteID);	
		}
		
		if(GUI.Button( new Rect(10, y + 10, 100, 20), "+"))
		{
			Tile newTile = manager.AddTile();
			newTile.TextureID = "sdf";
		}
		
		GUI.EndScrollView();
	}
	
}
