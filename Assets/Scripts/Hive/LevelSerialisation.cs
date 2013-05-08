using System;
using System.IO;
using System.Text;
using System.Xml;
//using System.Text.
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
		
		TextAsset levelAsset = AssetHelper.Instance.GetAsset<TextAsset>(path) as TextAsset;
		if(levelAsset != null)
		{
			levelDoc.LoadXml(levelAsset.text);
		}
		else
		{
			Debug.Log("Failed to load level: " + path);
			return;	
		}
		
		LoadXml(levelDoc);
	}
	
	private void LoadXml(XmlDocument levelDoc)
	{
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
		
		// Now that the data is loaded, the collision data can be correctly determined
		foreach(var section in m_sections)
		{
			section.RebuildColliders();
		}
		
		m_graph = BuildAIGraph();
	}
	
	[RPC]
	public void SpawnNPC(Vector3 location, NetworkViewID networkID)
	{
		if(m_npcObject != null)
		{
			GameObject npc = GameObject.Instantiate(m_npcObject, location, Quaternion.identity) as GameObject;
			npc.transform.position = location + new Vector3(0.0f, 0.0f, -1.0f);
			
			// Shut down the AI on the admin-side.
			// TODO: Generalise this to an admin/agent general call.
			AIWander npcWander = npc.GetComponent<AIWander>();
			npcWander.enabled = false;
		}
		else
		{
			Debug.LogError("NPC object is null. Spawn failed");	
		}
	}
	
	[RPC]
	public void ReceiveLevel(byte[] levelData)
	{
		Debug.LogWarning("Received RPC data");
		//String levelString = System.Text.Encoding.UTF8.GetString(levelData);
		//XmlDocument levelDoc = new XmlDocument();
		
		Seed = BitConverter.ToInt32(levelData, 0);
		
		LevelGen generator = new LevelGen(this);
		generator.GenerateLevel(Seed, false);
		//levelDoc.LoadXml(levelString);
		
		//LoadXml(levelDoc);
	}
	
	public void SerialiseToNetwork(string path)
	{
		Debug.Log("Serialising RPC data...");
		TextAsset levelAsset = Resources.Load(path) as TextAsset;
		if(levelAsset != null)
		{
			//byte[] bytes = System.Text.Encoding.UTF8.GetBytes(levelAsset.text);
			byte[] bytes = BitConverter.GetBytes(Seed);
			
			networkView.RPC("ReceiveLevel", RPCMode.Others, bytes);
		}
		else
		{
			Debug.Log("Failed to serialise level: " + path);
			return;	
		}
		
		Debug.Log("Done");
	}
	
	public void SyncNPCs()
	{
		return;
		/*
		if(m_npcObject != null)
		{
			GameObject testNPC = GameObject.Instantiate(m_npcObject) as GameObject;	
			testNPC.transform.position = (Vector3)m_graph.Nodes[0].NodePosition + new Vector3(0.0f, 0.0f, -1.0f);
			
			networkView.RPC ("SpawnNPC", RPCMode.Others, testNPC.transform.position, testNPC.networkView.viewID);
		}
		*/
	}
}