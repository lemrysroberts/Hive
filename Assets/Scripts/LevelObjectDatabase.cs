/// <summary>
/// Not really a database in any strict sense. Rather a place to keep persistent data not used by the Level.
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public class LevelObjectDatabase : MonoBehaviour 
{
	public List<LevelObject> LevelObjects
	{
		get { return m_levelObjects; }
		set { m_levelObjects = value; }
	}
	
	public string LastError
	{
		get { return m_lastError; }	
	}
	
	public LevelObject GetObject(string objectName)
	{
		foreach(var levelObject in m_levelObjects)
		{
			if(levelObject.Name == objectName)
			{
				return levelObject.Clone() as LevelObject;
			}
		}
		return null;
	}
	
	public LevelObject AddObject(string objectName)
	{
		LevelObject newObject = null;
		
		foreach(var levelObject in m_levelObjects)
		{
			if(levelObject.Name == objectName)
			{
				m_lastError	= "Unable to add \"" + objectName + "\". An object with that name already exists.";
				return null;
			}
		}
		
		newObject = new LevelObject(objectName);	
		m_levelObjects.Add(newObject);
		
		return newObject;
	}
	
	[SerializeField]
	private List<LevelObject> m_levelObjects = new List<LevelObject>();
	
	private string m_lastError = string.Empty;
}
