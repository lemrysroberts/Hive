using UnityEngine;
using System.Collections;
using System.Xml;

public class Tile 
{

	public int ID {	get; set; }
	public string TextureID	{ get; set;	}
	
	private Texture m_texture;
	private Material m_material;
	
	public void Save(XmlTextWriter writer)
	{
		writer.WriteStartElement("tile");
		
		writer.WriteAttributeString("id", ID.ToString());
		writer.WriteAttributeString("texture_id", TextureID);
		
		writer.WriteEndElement(); // tile
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
			// TODO: Extract the path	
		}
	}
	
	public void LoadResources()
	{
		SetTexture( AssetHelper.Instance.GetAsset<Texture>(TextureID) as Texture );
	}
	
	public Material GetMaterial()
	{
		return m_material;	
	}
}
