using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelSection : MonoBehaviour, IVisibilityReceiver
{
	public void Save(XmlTextWriter writer)
	{
		writer.WriteStartElement("section");
		
		writer.WriteAttributeString("origin_x", m_origin.x.ToString());
		writer.WriteAttributeString("origin_y", m_origin.y.ToString());
		
		foreach(var tileID in m_tileIDs)
		{
			writer.WriteStartElement("tile");
			
			writer.WriteAttributeString("id", tileID.ToString());
			
			writer.WriteEndElement(); // tile
		}
		
		writer.WriteEndElement(); // section
	}
	
	public void Load(XmlNode sectionNode)
	{
		string originXString = sectionNode.Attributes.GetNamedItem("origin_x").Value;
		string originYString = sectionNode.Attributes.GetNamedItem("origin_y").Value;
		
		float.TryParse(originXString, out m_origin.x);
		float.TryParse(originYString, out m_origin.y);
		
		m_tileIDs = new List<int>(m_sectionSize * m_sectionSize);
		for(int i = m_tileIDs.Count; i < m_sectionSize * m_sectionSize; i++) m_tileIDs.Add(0);
		
		int tileIndex = 0;
		foreach(XmlNode tileNode in sectionNode.ChildNodes)
		{
			int tileID = 0;
			
			int.TryParse(tileNode.Attributes.GetNamedItem("id").Value, out tileID);
			m_tileIDs[tileIndex] = tileID;
			
			tileIndex++;
		}
		
		RebuildData();
	}
}
