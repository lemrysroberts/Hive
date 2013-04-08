using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Level))]
public class LevelGen : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		m_level = GetComponent<Level>();
		
		int roomCount = 5;
		m_rooms = new Room[roomCount];
		
		int width = m_level.SectionCountX * Level.m_sectionSize;
		int height = m_level.SectionCountY * Level.m_sectionSize;
		
		
		for(int i = 0; i < roomCount; i++)
		{
			int startX 	= (int)(Random.value * (width - 30));
			int startY 	= (int)(Random.value * (height - 30));
			
			for(int x = startX; x < startX + 30; x++)
			{
				for(int y = startY; y < startY + 30; y++)
				{
					m_level.SetTileID(x, y, 1, false);
				}
			}
			m_rooms[i] = new Room();
			
			m_rooms[i].startX = startX;
			m_rooms[i].endX = startX + 30;
			m_rooms[i].startY = startY;
			m_rooms[i].endY = startY + 30;;
			
		}
		
		
		m_level.RebuildAllSections();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private Level m_level;
	private Room[] m_rooms;
	
	private class Room
	{
		public int startX = 0;
		public int startY = 0;
		public int endX = 0;
		public int endY = 0;
	}
}
