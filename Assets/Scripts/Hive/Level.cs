using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NavState
{
	Open,
	LayoutBlocked,
	ThingBlocked
}

[ExecuteInEditMode]
public partial class Level : MonoBehaviour 
{
	void Start()
	{
		m_previousSectionCountX = SectionCountX;	
		m_previousSectionCountY = SectionCountY;
	}
	
	void OnEnable()
	{
		//Reload(true);	
	}
	
	public void Reload(bool fullReload)
	{
		DeleteSections(fullReload);
		Transform sectionsTransform =  CreateSectionsObject().transform;
		
		List<LevelSection> newSections = new List<LevelSection>();
		for(int i = 0; i < SectionCountX * SectionCountY; i++) newSections.Add(null);
		
		{
			for(int x = 0; x < SectionCountX; x++)
			{
				for(int y = 0; y < SectionCountY; y++)
				{
					if( x < m_previousSectionCountX && 
						y < m_previousSectionCountY && 
						x < SectionCountX &&
						y < SectionCountY &&
						m_sections.Count > 0)
					{
						newSections[x * SectionCountY + y] = m_sections[x * m_previousSectionCountY + y];	
					}
					
					bool sectionExists = false;
					if(!fullReload)
					{
						for(int childIndex = 0; childIndex < sectionsTransform.GetChildCount(); childIndex++)
						{
							LevelSection otherSection = sectionsTransform.GetChild(childIndex).gameObject.GetComponent<LevelSection>();
							if(otherSection.Origin.x == m_sectionSize * x && otherSection.Origin.y == m_sectionSize * y)
							{
								sectionExists = true;
							}
						}
					}
					
					if(sectionExists)
					{
						continue;	
					}
						
					GameObject newSection = new GameObject("Section " + x + ", " + y);
					newSection.transform.parent = sectionsTransform;
					
					
					
					LevelSection section = newSection.AddComponent<LevelSection>();
					
					newSection.tag = "level_section";
			
					BoxCollider collider 	= newSection.AddComponent<BoxCollider>();
					Rigidbody rigidBody 	= newSection.AddComponent<Rigidbody>(); // I hate this
					m_visReporter 			= newSection.AddComponent<VisibilityReporter>();
					m_visReporter.RegisterReceiver(section);
					
					rigidBody.isKinematic = true;
					
					collider.isTrigger = true;
					collider.center = new Vector3(m_sectionSize / 2, m_sectionSize / 2, 0.0f);
					collider.size 	= new Vector3(m_sectionSize, m_sectionSize, 10.0f); // The 5.0f is fairly arbitrary
					
					section.Origin = new Vector2(x * m_sectionSize, y * m_sectionSize);
					section.SectionSize = m_sectionSize;
					section.Tile = TileType;
					section.RebuildData();
					
					newSections[x * SectionCountY + y] = section;
				}
			}
			
			m_sections = newSections; 
		}
		
		
		m_previousSectionCountX = SectionCountX;
		m_previousSectionCountY = SectionCountY;
	}
	
	public void RebuildAllSections()
	{
		foreach(var section in m_sections)
		{
			section.RebuildData();
		}
	}
	
	public void SetTileID(int x, int y, int tileID, bool rebuild)
	{
		int sectionIDX = x / m_sectionSize;
		int sectionIDY = y / m_sectionSize;
		
		int localIDX = x % m_sectionSize;
		int localIDY = y % m_sectionSize; 
		
		LevelSection section = m_sections[sectionIDX * SectionCountY + sectionIDY];
		
		section.TileIDs[localIDX * m_sectionSize + localIDY] = tileID; 
//		section.NavStates[localIDX * m_sectionSize + localIDY] = TileManager.Instance.GetTile(tileID).NavBlock ? NavState.LayoutBlocked : NavState.Open;
		
		if(rebuild)
			section.RebuildData();
	}
	
	private void DeleteSections(bool fullReload)
	{
		Transform sectionsTransform = transform.FindChild(s_sectionsID);
		
		if(sectionsTransform != null)
		{
			List<LevelSection> toDelete = new List<LevelSection>();
			
			for(int childIndex = 0; childIndex < sectionsTransform.GetChildCount(); childIndex++)
			{
				LevelSection section = sectionsTransform.GetChild(childIndex).gameObject.GetComponent<LevelSection>();
				if(section.Origin.x >= m_sectionSize * SectionCountX || section.Origin.y >= m_sectionSize * SectionCountY || fullReload)
				{
					toDelete.Add(section);	
				}
			}
			
			foreach(var section in toDelete) 
			{
				DestroyImmediate(section.gameObject);	
			}	
		}
	}
	
	private GameObject CreateSectionsObject()
	{
		Transform sectionsTransform = transform.FindChild(s_sectionsID);
		
		if(sectionsTransform == null)
		{
			GameObject sectionsObject = new GameObject(s_sectionsID);
			sectionsTransform = sectionsObject.transform;
			sectionsTransform.parent = transform;
		}
		
		return sectionsTransform.gameObject;
	}
	
	void OnDrawGizmos()
	{
		if(m_sections != null)
		{
			foreach(var section in m_sections)
			{
				if(section != null)
				{
					section.OnDrawGizmos();	 
				}
			}
		}
	}
		
	public void OnGUI()
	{
		int sections = 0;
		foreach(var section in m_sections)
		{
			if(section.m_visibleSections > 0)
			{
				sections++;	
			}
		}
		
		GUI.Label(new Rect(0, 0, 300, 300), "Updating " + sections + " sections");	
	}
	
	public NavState GetNavState(int x, int y)
	{
		int sectionIDX = x / m_sectionSize;
		int sectionIDY = y / m_sectionSize;
		
		int localIDX = x % m_sectionSize;
		int localIDY = y % m_sectionSize; 
		
		LevelSection section = m_sections[sectionIDX * SectionCountY + sectionIDY];
		
		return section.NavStates[localIDX * m_sectionSize + localIDY]; 
	}
	
	public void SetNavState(int x, int y, NavState state)
	{
		int sectionIDX = x / m_sectionSize;
		int sectionIDY = y / m_sectionSize;
		
		int localIDX = x % m_sectionSize;
		int localIDY = y % m_sectionSize; 
		
		LevelSection section = m_sections[sectionIDX * SectionCountY + sectionIDY];
		
		section.NavStates[localIDX * m_sectionSize + localIDY] = state; 
	}

	
	public GameObject TileType;
	public int SectionCountX = 30;
	public int SectionCountY = 30;
	
	public static int m_sectionSize = 30;
	
	[SerializeField]
	public List<LevelSection> m_sections = new List<LevelSection>();
	
	private const string s_sectionsID = "sections";
	
	[SerializeField]
	private int m_previousSectionCountX = 30;
	
	[SerializeField]
	private int m_previousSectionCountY = 30;
	
	[SerializeField]
	private VisibilityReporter m_visReporter = null;
}
