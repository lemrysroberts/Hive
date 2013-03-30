using System;
using System.Text;
using System.Xml;
using UnityEngine;

public partial class Level : MonoBehaviour 
{
		
	public void Save(string path)
	{
		using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.Unicode))
		{
			writer.Formatting = Formatting.Indented;
			
			writer.WriteStartDocument();
			
			writer.WriteStartElement("level");
			
			writer.WriteAttributeString("section_size", m_sectionSize.ToString());
			writer.WriteAttributeString("sections_x", SectionCountX.ToString());
			writer.WriteAttributeString("sections_y", SectionCountY.ToString());
			
			writer.WriteStartElement("sections");
			
			foreach(var section in m_sections)
			{
				section.Save(writer);
			}
			
			writer.WriteEndElement(); // sections
			
			writer.WriteEndElement(); // level
			
			writer.WriteEndDocument();
		}
	}
	
	public void Load(string path)
	{
		XmlDocument levelDoc = new XmlDocument();
		levelDoc.Load(path);
		
		XmlNodeList levelNodes = levelDoc.GetElementsByTagName("level");
		if(levelNodes.Count == 0) return;
		
		string sectionSizeString = levelNodes[0].Attributes.GetNamedItem("section_size").Value;
		string sectionXString = levelNodes[0].Attributes.GetNamedItem("sections_x").Value;
		string sectionYString = levelNodes[0].Attributes.GetNamedItem("sections_y").Value;
		
		bool parsed = false;
		parsed |= int.TryParse(sectionSizeString, out m_sectionSize);
		parsed |= int.TryParse(sectionXString, out SectionCountX);
		parsed |= int.TryParse(sectionYString, out SectionCountY);
		
		Reload(true);
		
		XmlNodeList sections = levelDoc.GetElementsByTagName("section");
		int sectionID = 0;
		foreach(XmlNode sectionNode in sections)
		{
			m_sections[sectionID].SectionSize = m_sectionSize;
			m_sections[sectionID].Load(sectionNode);	
			sectionID++;
		}
	}
}