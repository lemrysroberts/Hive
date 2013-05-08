/// <summary>
/// Tile.
/// 
/// TODO: Make XML IDs individual const strings.
/// 
/// </summary>

#if UNITY_EDITOR
	using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Xml;

public class Tile 
{
#region Properties
	public int ID 					{ get; set; }
	public string TextureID			{ get; set;	}
	public bool Animated 			{ get; set; }
	public string SpriteDataPath 	{ get; set; }
	public bool NavBlock 			{ get; set; }
	public float Elevation			{ get; set; }
	public float AnimationSpeed		{ get; set; }
#endregion

#region Fields
	private Texture m_texture;
	private Material m_material;
#endregion
	
	/// <summary>
	/// Saves the tile. 
	/// This should only be called from the TileManager.
	/// </summary>
	public void Save(XmlTextWriter writer)
	{
		writer.WriteStartElement("tile");
		writer.WriteAttributeString("id", 		ID.ToString());
		writer.WriteAttributeString("texture_id", TextureID);
		writer.WriteAttributeString("nav_block", NavBlock.ToString());
		
		if(SpriteDataPath != null)
		{
			writer.WriteAttributeString("sprite_data_path", SpriteDataPath);
		}
		
		writer.WriteAttributeString("elevation", Elevation.ToString());
		writer.WriteAttributeString("animation_speed", AnimationSpeed.ToString());
		
		writer.WriteEndElement(); 
	} 
	
	/// <summary>
	/// Loads a tile from the given XML.
	/// </summary>
	public void Load(XmlNode node)
	{
		XmlNode spritePathNode = node.Attributes.GetNamedItem("sprite_data_path");
		XmlNode navBlockNode = node.Attributes.GetNamedItem("nav_block");
		XmlNode elevationNode = node.Attributes.GetNamedItem("elevation");
		XmlNode animationSpeedNode = node.Attributes.GetNamedItem("animation_speed");
		
		if(spritePathNode != null) 	
		{ 
			SpriteDataPath = spritePathNode.Value;	
		}
		
		if(navBlockNode != null) 	
		{ 
			bool navBlockOut = false;
			bool.TryParse(navBlockNode.Value, out navBlockOut); 
			NavBlock = navBlockOut;
		}
		
		if(elevationNode != null)
		{
			float elevation = 0.0f;
			float.TryParse(elevationNode.Value, out elevation);
			Elevation = elevation;
		}
		
		if(animationSpeedNode != null)
		{
			float animationSpeed = 0.0f;
			float.TryParse(animationSpeedNode.Value, out animationSpeed);
			AnimationSpeed = animationSpeed;
		}
		
		string idNode = node.Attributes.GetNamedItem("id").Value;
		TextureID = node.Attributes.GetNamedItem("texture_id").Value;
		
		int newID = -1;
		bool IDFound = int.TryParse(idNode, out newID);
		if(!IDFound)
		{
			Debug.LogError("Valid ID not found for tile: " + idNode + " | " + TextureID);	
		}
		
		ID = newID; 
	}
	
	/// <summary>
	/// Gets the tile's texture.
	/// </summary>
	public Texture GetTexture()
	{
		return m_texture;	
	}
	
	/// <summary>
	/// Sets the tile's texture and builds a material from that texture using the global tile-material from the TileManager.
	/// </summary>
	public void SetTexture(Texture newTexture)
	{
		if(newTexture != m_texture)
		{
			if(m_material == null || TileManager.TileMaterial == null)
			{
				m_material = new Material(TileManager.TileMaterial);
			}
			
			m_texture 				= newTexture;
			m_material.mainTexture 	= m_texture;
			
#if UNITY_EDITOR
			// In the editor, store the path of the given texture for use when saving the tile-set.
			TextureID = AssetDatabase.GetAssetPath(newTexture);
			TextureID = TextureID.Remove(0, "Assets/Resources/".Length);
#endif
		}
	}
	
	/// <summary>
	/// Loads the tile's texture from its given texture-ID.
	/// </summary>
	public void LoadResources()
	{
		SetTexture( AssetHelper.Instance.GetAsset<Texture>(TextureID) as Texture );
	}
	
	/// <summary>
	/// Gets the material of the tile, comprised of the material from the TileManager and the tile's texture.
	/// </summary>
	public Material GetMaterial()
	{
		// This should only be necessary in the editor.
		// On return from run-in-editor, the material is flushed but the TileManager singleton is not.
		// Force a refresh.
		if(m_material == null)
		{
			if(TileManager.TileMaterial != null)
			{
				m_material = new Material(TileManager.TileMaterial);
				m_material.mainTexture = m_texture;
			}
		}
		
		return m_material;	
	}
}
