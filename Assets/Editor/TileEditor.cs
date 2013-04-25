/// <summary>
/// Tile editor.
/// 
/// This is a rather rough interface through to the TileManager.
/// 
/// It effectively has two states that it can be displaying at any time:
/// 
/// * 	The general purpose tile-selection state. This shows a list of all available tiles and details
/// 	on the currently selected tile to paint.
/// 
/// * 	The TileManager is fairly thick, so it just keeps a max-index of Tile IDs used. This means that
/// 	when a tile is added, it will just be assigned the maximum index of all tiles.
/// 	This is all well and good until you accidentally delete the tile used for 90% of the map.
/// 	That tile's ID was used in all the level data, but is now defunct. The alternate editor state allows
/// 	the selection of previously used IDs, so the user can reactivate that ID. 
/// 	The interface for this state is double-ugly.
/// 
/// </summary>

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileEditor : EditorWindow
{
	/// <summary>
	/// Shows the editor window.
	/// </summary>
	[MenuItem("Tiles/Tile Manager")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(TileEditor));
	}
	
	void OnGUI()
	{
		// Display the ID-request screen instead of the normal tile-select
		if(m_displayingIDRequest )
		{
			RequestID();
			return;	
		}
		
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Save", GUILayout.Width(100)))
		{
			string tileset = EditorUtility.SaveFilePanel("Save Tileset", "Resources", "tiles", "xml");
			if(tileset != null && tileset != string.Empty)
			{
				TileManager.Instance.Save(tileset);
			}
		}
		
		if(GUILayout.Button("Load", GUILayout.Width(100)))
		{
			string tileset = EditorUtility.OpenFilePanel("Load Tileset", "Resources", "xml");	
			if(tileset != null && tileset != string.Empty)
			{
				TileManager.TileSetFilename = tileset;
				TileManager.Instance.Init();	
			}
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		DrawTileList();
		GUILayout.Box("", GUILayout.Height(position.height - 30), GUILayout.Width(1));
		
		DrawDetails();
		
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Draws the list of all tiles in the currently selected tile-set.
	/// </summary>
	private void DrawTileList()
	{
		TileManager manager = TileManager.Instance;
	
		m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(m_listWidth));
		List<Tile> toDelete = new List<Tile>();
		
		foreach(var tilePair in manager.Tiles)
		{
			Tile tile = tilePair.Value;
			
			// Don't show the debug tile
			if(tile.ID == -1)
			{
				continue;
			}
			
			GUILayout.BeginHorizontal();
			
			tile.SetTexture( EditorGUILayout.ObjectField( tile.GetTexture(), typeof(Texture), false, GUILayout.Width(40), GUILayout.Height(40)) as Texture);
			
			GUILayout.BeginVertical(GUILayout.Width(100));
			
			if(GUILayout.Button("Select", GUILayout.Width(80)))
			{
				TileManager.Instance.SelectedTile = tile;	
			}
			
			if(GUILayout.Button("Delete", GUILayout.Width(80)))
			{
				toDelete.Add(tile);
			}
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndHorizontal();
			GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(120));
		}
		
		if(GUILayout.Button("+"))
		{
			List<int> unusedIDs = manager.GetUnusedIDs();
			if(unusedIDs.Count > 0)
			{
				m_displayingIDRequest = true;
				
				m_idStrings = new string[unusedIDs.Count];
				int count = 0;
				foreach(var id in unusedIDs)
				{
					m_idStrings[count] = id.ToString();
					count++;
				}
			}
			else
			{
				Tile newTile = manager.AddTile();
				newTile.TextureID = "New Texture";
			}
		}
		
		foreach(var tile in toDelete)
		{
			manager.RemoveTile(tile.ID);	
		}
		
		GUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Draws the details of the TileManager's currently selected tile.
	/// </summary>
	private void DrawDetails()
	{
		TileManager manager = TileManager.Instance;
		
		Tile tile = manager.SelectedTile;
		
		if(tile != null)
		{
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
	}
	
	/// <summary>
	/// Draws the list of unused IDs used when adding a new tile.
	/// </summary>
	void RequestID()
	{
		TileManager manager = TileManager.Instance;
		
		m_unusedIDScrollPos = GUILayout.BeginScrollView(m_unusedIDScrollPos, GUILayout.Width(200));
		
		m_selectedID = GUILayout.SelectionGrid(m_selectedID, m_idStrings, 1);
		
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("New ID", GUILayout.Width(100)))
		{
			Tile newTile 			= manager.AddTile();
			newTile.TextureID 		= "New Texture";
			m_displayingIDRequest 	= false;
		}
		
		if(GUILayout.Button("Use Selected ID", GUILayout.Width(100)))
		{
			int id = -1;
			int.TryParse(m_idStrings[m_selectedID], out id);
			
			manager.AddTile(id);
			
			m_displayingIDRequest = false;
		}
		
		GUILayout.EndHorizontal();
	}
	
	private string[] m_idStrings;
	private int m_selectedID 			= 0;
	private bool m_displayingIDRequest 	= false;
	private const int m_listWidth 		= 160;
	
	Vector2 m_scrollPos			= Vector2.zero;
	Vector2 m_unusedIDScrollPos = Vector2.zero;
}
