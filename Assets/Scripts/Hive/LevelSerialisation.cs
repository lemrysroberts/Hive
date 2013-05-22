using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
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
		Seed = BitConverter.ToInt32(levelData, 0);
		
		LevelGenerator generator = new LevelGenerator(this);
		generator.GenerateLevel(Seed, false);
		
		SetLevelObjectIDs();
	}
	
	[RPC]
	public void SetLevelObjectViewID(int objectID, NetworkViewID networkID)
	{
		m_levelObjectIDs.Add(objectID, networkID);	
	}
	
	[RPC]
	public void ClearLevelObjectIDs()
	{
		m_levelObjectIDs.Clear();	
	}
	
	[RPC]
	public void BindLevelObjectIDs()
	{
		SetLevelObjectIDs();
	}
	
	public void SerialiseToNetwork(string path)
	{
		Debug.Log("Serialising RPC data...");
		TextAsset levelAsset = Resources.Load(path) as TextAsset;
		if(levelAsset != null)
		{
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
	
	/// <summary>
	/// This bungs a big list of LevelObject-ID/NetworkViewID pairs across to the other player to let them know how to wire their network gubbins up.
	/// TODO:   When my brain was being less stupid-bad, it did occur to me that the order in which LevelObjects are instantiated probably makes
	///  	 	this completely pointless. If the objects are instantiated in consistent order on both machines, they will (probably) be allocated
	/// 		matching NetworkViewIDs.
	/// 		However! That idea falls down if either view uses a NetworkViewID before the objects are instantiated, as the allocations will mis-match. However, however, why would one
	/// 		machine be instantiating network-data that isn't matched on the other? Misses the point of networking, rather...
	/// 		Conclusion: Better safe than sorry.
	/// </summary>
	public void SerialiseLevelObjectIDs()
	{
		m_levelObjectIDs.Clear();
		foreach(var levelObject in m_levelObjects)
		{
			NetworkViewID id;
			if(levelObject.GetNetworkViewID(out id))
			{
				m_levelObjectIDs.Add(levelObject.ID, id);	
			}
		}
		
		// Make sure the other chap has flushed out any old lists of IDs
		networkView.RPC("ClearLevelObjectIDs", RPCMode.Others);
		
		// Chuck all them lovely new IDs across.
		foreach(var id in m_levelObjectIDs)
		{
			networkView.RPC("SetLevelObjectViewID", RPCMode.Others, id.Key, id.Value);	
		}
		
		// Tell the bugger to re-bind all of his objects to their shiny new IDs.
		networkView.RPC("BindLevelObjectIDs", RPCMode.Others);
	}
	
	/// <summary>
	/// Binds all LevelObject instances to the networkViewIDs currently stored for them.
	/// </summary>
	private void SetLevelObjectIDs()
	{
		if(m_levelObjectIDs != null)
		{
			foreach(var levelObject in m_levelObjects)
			{
				NetworkViewID id;
				if(m_levelObjectIDs.TryGetValue(levelObject.ID, out id))
				{
					levelObject.SetNetworkViewID(id);
				}
			}
		}
	}
}