using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class TileManager 
{
#region Properties
	public static string TileSetFilename { get; set; }
	
	public Dictionary<int, Tile> Tiles
	{
		get { return m_tiles; }	
	}
	
	public Tile SelectedTile { get; set; }
	
	public static Material TileMaterial
	{
		get 
		{
			if(m_tileMaterial == null)
			{
				m_tileMaterial = AssetHelper.Instance.GetAsset<Material>(m_tileMaterialID) as Material;
			}
			return m_tileMaterial;
		}
	}
#endregion

#region Fields
	private static Material m_tileMaterial 	= null;
	private static TileManager s_instance 	= null;	
	
	private Dictionary<int, Tile> m_tiles	= new Dictionary<int, Tile>();
	private int m_maxFreeID 				= 0;
	private bool m_initialised 				= false;
	private const string m_tileMaterialID 	= "Materials/tilematerial";
#endregion	
	
#region Methods
	/// <summary>
	/// Singleton accessor.
	/// </summary>
	public static TileManager Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new TileManager();	
				s_instance.Init();
			}
			
			return s_instance;
		}
	}
	
	/// <summary>
	/// Loads all tiles.
	/// </summary>
	public void Init()
	{
		Reset ();
		if(!m_initialised)
		{	
			if(TileSetFilename == null)
			{
				TileSetFilename = "tileset";	
			}
			
			m_initialised = true;
			Load(TileSetFilename);
		}
	}
	
	/// <summary>
	/// Gets a given tile from a particular ID.
	/// </summary>
	public Tile GetTile(int tileID)
	{
		Tile returnTile = null;
		m_tiles.TryGetValue(tileID, out returnTile);
		return returnTile;
	}
	
	/// <summary>
	/// Adds a new empty tile and increments the maximum ID.
	/// </summary>
	public Tile AddTile()
	{
		int newID = m_maxFreeID;
		m_maxFreeID++;
		
		Tile newTile = new Tile();
		newTile.ID = newID;
		
		m_tiles.Add(newID, newTile);
		return newTile;
	}
	
	/// <summary>
	/// Removes a given tile.
	/// </summary>
	public void RemoveTile(int id)
	{
		m_tiles.Remove(id);	
	}
	
	/// <summary>
	/// Serialises the TileManager data and calls Save on each tracked Tile.
	/// </summary>
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
	
	/// <summary>
	/// Loads a tile-set from the given resource-path.
	/// </summary>
	public void Load(string tileDatabasePath)
	{
		XmlDocument doc = new XmlDocument();
		TextAsset tileText = AssetHelper.Instance.GetAsset<TextAsset>(tileDatabasePath) as TextAsset;
		
		if(tileText != null)
		{
			doc.LoadXml(tileText.text);
		}
		else
		{
			Debug.LogError("Tileset data not found: " + tileDatabasePath);	
		}
		
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
	
	/// <summary>
	/// Resets the TileManager to an empty state.
	/// </summary>
	private void Reset()
	{
		m_tiles.Clear();
		m_initialised = false;
		m_maxFreeID = 0;
		SelectedTile = null;
	}
#endregion
	
}
