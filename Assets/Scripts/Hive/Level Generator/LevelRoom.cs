using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Room
{
	public List<Vector2> tileIDs = new List<Vector2>();
	
	
	public int startX 			= 0;
	public int startY 			= 0;
	public int endX 			= 0;
	public int endY				= 0;
	
	public List<Room> ConnectedRooms { get { return m_connectedRooms; } }
	private List<Room> m_connectedRooms = new List<Room>();
	/*
	public bool Overlaps(Room other)
	{
		if(other == null)
		{
			return false;	
		}
		
		return !(other.startX > endX || other.startY > endY || other.endX < startX || other.endY < startY);
	}
	*/
}
