using UnityEngine;
using System.Xml;
using System.Collections;

public class LevelObject : MonoBehaviour 
{
	public string PrefabName = string.Empty;
	public SaveSerialisable SerialiseTarget = null;
	
	// Use this for initialization
	void Start () 
	{
		Level level = FindObjectOfType(typeof(Level)) as Level;
	}
	
	public void Serialise(XmlTextWriter writer)
	{
		writer.WriteStartElement("object");
		
		writer.WriteAttributeString("prefab_name", PrefabName);
		
		//writer.WriteAttributeString("position
		
		if(SerialiseTarget != null)
		{
			SerialiseTarget.SaveSerialise(writer);	
		}
		
		writer.WriteEndElement();
	}
}
