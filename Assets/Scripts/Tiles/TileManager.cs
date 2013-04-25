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
	private Texture m_debugTex;
	private Tile m_debugTile				= null;
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
			m_debugTex = AssetHelper.Instance.GetAsset<Texture>("textures/debug") as Texture;
			
			m_debugTile = new Tile();
			m_debugTile.Animated = false;
			m_debugTile.ID = -1;
			m_debugTile.SetTexture(m_debugTex);
			
			m_tiles.Add(-1, m_debugTile);
			
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
		
		if(returnTile == null)
		{
			returnTile = m_debugTile;
		}
		
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
	
	public Tile AddTile(int id)
	{
		Tile newTile = new Tile();
		newTile.ID = id;
		
		m_tiles.Add(id, newTile);
		return newTile;
	}
	
	/// <summary>
	/// Removes a given tile.
	/// </summary>
	public void RemoveTile(int id)
	{
		m_tiles.Remove(id);	
		
		if(id == m_maxFreeID - 1)
		{
			m_maxFreeID = id;
		}
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
		Reset(); 
		
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
	
	public List<int> GetUnusedIDs()
	{
		List<int> unusedIDs = new List<int>();
		
		for(int i = 0; i < m_maxFreeID; i++)
		{
			if(!m_tiles.ContainsKey(i))
			{
				unusedIDs.Add(i);	
			}
		}
		
		return unusedIDs;
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
