/// <summary>
/// Not really a database in any sense. Rather a place to keep persistent data not used by the Level.
/// 
/// Update: Now ghettoised to store values used by the level-generator. 
/// </summary>

using UnityEngine;
using System;
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
		
		Debug.LogWarning("Failed to locate \"" + objectName + "\" in the LevelObject database. Returning null");
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
	
	public void AddEditorObject(string key, GameObject value)
	{
		foreach(var pair in m_editorPairs)
		{
			if(pair.Key == key)
			{
				pair.Value = value;
				return;
			}
		}
		
		KeyValuePair newPair = new KeyValuePair();
		newPair.Key = key;
		newPair.Value = value;
		
		m_editorPairs.Add(newPair);
	}
	
	public GameObject GetEditorObject(string key)
	{
		foreach(var pair in m_editorPairs)
		{
			if(pair.Key == key)
			{
				return pair.Value;
			}
		}
		return null;
	}
	
	[SerializeField]
	private List<LevelObject> m_levelObjects = new List<LevelObject>();
	
	[SerializeField]
	private List<KeyValuePair> m_editorPairs = new List<KeyValuePair>();
	
	private string m_lastError = string.Empty;
	
	// Unity can't serialize dictionaries, so blah.
	[Serializable]
	private class KeyValuePair
	{
		[SerializeField]
		public string Key;
		
		[SerializeField]
		public GameObject Value;
	}		
}
