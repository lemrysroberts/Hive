using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;

public class Tile 
{
	public int ID {	get; set; }
	public string TextureID	{ get; set;	}
	public bool Animated { get; set; }
	public string SpriteDataPath { get; set; }
	
	private Texture m_texture;
	private Material m_material;
	
	public void Save(XmlTextWriter writer)
	{
		writer.WriteStartElement("tile");
		
		writer.WriteAttributeString("id", ID.ToString());
		writer.WriteAttributeString("texture_id", TextureID);
		if(SpriteDataPath != null) writer.WriteAttributeString("sprite_data_path", SpriteDataPath);
		
		writer.WriteEndElement(); // tile
	} 
	
	public void Load(XmlNode node)
	{
		XmlNode spritePathNode = node.Attributes.GetNamedItem("sprite_data_path");
		if(spritePathNode != null)
		{
			SpriteDataPath = spritePathNode.Value;	
		}
		
		string idNode = node.Attributes.GetNamedItem("id").Value;
		TextureID = node.Attributes.GetNamedItem("texture_id").Value;
		
		int newID = -1;
		int.TryParse(idNode, out newID);
		
		ID = newID;
	}
	
	public Texture GetTexture()
	{
		return m_texture;	
	}
	
	public void SetTexture(Texture newTexture)
	{
		if(newTexture != m_texture)
		{
			if(m_material == null || TileManager.TileMaterial == null)
			{
				m_material = new Material(TileManager.TileMaterial);
			}
			
			m_texture = newTexture;
			m_material.mainTexture = m_texture;
			
			TextureID = AssetDatabase.GetAssetPath(newTexture);
		}
	}
	
	public void LoadResources()
	{
		SetTexture( AssetHelper.Instance.GetAsset<Texture>(TextureID) as Texture );
	}
	
	public Material GetMaterial()
	{
		// This should only be necessary in the editor.
		// On return from run-in-editor, the material is flushed but the TileManager singleton is not.
		// Force a refresh.
		if(m_material == null)
		{
			TileManager.Instance.ReloadTileMaterial();
			m_material = new Material(TileManager.TileMaterial);
			m_material.mainTexture = m_texture;
		}
		
		return m_material;	
	}
}
