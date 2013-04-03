using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class TileManager 
{
	
	public static TileManager Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new TileManager();	
			}
			
			return s_instance;
		}
	}
	
	public void Init()
	{
		TileMaterial = AssetHelper.Instance.GetAsset<Material>("Assets/Materials/tilematerial.mat") as Material;
		
		Tile newTile1 = AddTile();
		Tile newTile2 = AddTile();
		
		newTile1.TextureID = "Assets/Textures/brick.png";
		newTile2.TextureID = "Assets/Textures/floor1.png";
		
		newTile1.LoadResources(); 
		newTile2.LoadResources();
		
		SelectedTile = newTile1;
	}
	
	public Tile GetTile(int tileID)
	{
		Tile returnTile = null;
		
		m_tiles.TryGetValue(tileID, out returnTile);
		
		return returnTile;
	}
	
	public Tile AddTile()
	{
		int newID = m_maxFreeID;
		m_maxFreeID++;
		
		Tile newTile = new Tile();
		newTile.ID = newID;
		
		m_tiles.Add(newID, newTile);
		
		return newTile;
	}
	
	public void RemoveTile(int id)
	{
		m_tiles.Remove(id);	
	}
	
	public Dictionary<int, Tile> Tiles
	{
		get { return m_tiles; }	
	}
	
	public Tile SelectedTile { get; set; }
	
	private TileManager() 
	{
		m_loaded = false;
		Init();
	}
	
	public void Save(string tileDatabasePath)
	{
		using(XmlTextWriter writer = new XmlTextWriter(tileDatabasePath, Encoding.Unicode))
		{
			writer.Formatting = Formatting.Indented;
			
			writer.WriteStartDocument();
			
			writer.WriteStartElement("tiles");
			
			foreach(var tile in m_tiles)
			{
				tile.Value.Save(writer);
			}
			
			writer.WriteEndElement(); // tiles
			
			writer.WriteEndDocument();
		}
	}
	
	public void Load(string tileDatabasePath)
	{
		
	}
	
	private static TileManager s_instance = null;	
	
	private bool m_loaded = false;
	
	private Dictionary<int, Tile> m_tiles = new Dictionary<int, Tile>();
	
	private int m_maxFreeID = 0;
	
	public static Material TileMaterial = null;
}
