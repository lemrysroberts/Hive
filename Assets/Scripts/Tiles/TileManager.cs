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
		ReloadTileMaterial();
		Load("C:/Unity/Hive/Assets/tileset.xml");
	}
	
	public void ReloadTileMaterial()
	{
		TileMaterial = AssetHelper.Instance.GetAsset<Material>("Assets/Materials/tilematerial.mat") as Material;
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
		XmlDocument doc = new XmlDocument();
		doc.Load(tileDatabasePath);
		
		XmlNodeList tileNodes = doc.GetElementsByTagName("tile");
		foreach(XmlNode tileNode in tileNodes)
		{
			Tile newTile = new Tile();
			newTile.Load(tileNode);
			newTile.LoadResources();
			
			m_tiles.Add(newTile.ID, newTile);
			
			m_maxFreeID = Mathf.Max(newTile.ID + 1, m_maxFreeID);
		}
		
		if(m_tiles.Count > 0)
		{
			SelectedTile = m_tiles[0];	
		}
	}
	
	private static TileManager s_instance = null;	
	
	private bool m_loaded = false;
	
	private Dictionary<int, Tile> m_tiles = new Dictionary<int, Tile>();
	
	private int m_maxFreeID = 0;
	
	public static Material TileMaterial = null;
}
