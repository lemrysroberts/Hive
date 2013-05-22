/// <summary>
/// A basic drop-down that builds its contents from a given search delegate.
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SearchDropdown 
{
	/// <summary>
	/// Search delegate for building the match-list.
	/// </summary>
	public delegate List<string> DropdownSearchHandler(string currentText);
	
	/// <summary>
	/// Used to draw the main text-field.
	/// </summary>
	/// <param name='currentString'>
	/// The string currently held by the field
	/// </param>
	/// <param name='handler'>
	/// A delegate for a search function, used to build a list of strings from a given fragment.
	/// </param>
	public static string OnGUI(string currentString, DropdownSearchHandler handler)
	{
		if(Event.current.type == EventType.KeyDown)
		{
			if(Event.current.keyCode == KeyCode.DownArrow && m_availableStrings.Count != 0)
			{
				if(m_selectedIndex < m_availableStrings.Count - 1)
				{
					m_selectedIndex++;
					currentString = m_availableStrings[m_selectedIndex];
				}
			}
			
			if(Event.current.keyCode == KeyCode.UpArrow  && m_availableStrings.Count != 0)
			{
				if(m_selectedIndex > -1)
				{
					m_selectedIndex--;	
					
					if(m_selectedIndex >= 0 && m_selectedIndex < m_availableStrings.Count)
					{
						currentString = m_availableStrings[m_selectedIndex];
					}
					else
					{
						currentString = m_lastString;
					}
				}
			}
			
			if(Event.current.keyCode == KeyCode.Escape || Event.current.keyCode == KeyCode.Return)
			{
				m_lastString = currentString;
				m_showDropDown = false;
			}
		}
		
		GUI.SetNextControlName(m_controlID);
		string newString = GUILayout.TextField(currentString);
		
		if(newString != currentString)
		{
			m_lastString = newString;
			
			m_availableStrings.Clear();
			if(handler != null && m_lastString != string.Empty)
			{
				m_availableStrings = handler(m_lastString);	
			}
			
			m_showDropDown = true;
		}
		
		if(Event.current.type == EventType.Repaint)
		{
			m_dropdownRect = GUILayoutUtility.GetLastRect();
		}
		
		if(GUI.GetNameOfFocusedControl() != m_controlID)
		{
			m_lastString = string.Empty;
			m_selectedIndex = -1;
		}
		
		return newString;
	}
	
	/// <summary>
	/// Draws the drop-down overlay on top of other components.
	/// Must be called after all elements it could overlap. This will probably always be last in practice.
	/// </summary>
	public static void DrawOverlay()
	{
		if(GUI.GetNameOfFocusedControl() == m_controlID && m_showDropDown && m_availableStrings.Count != 0)
		{
			Color current = GUI.color;
			Color newColor = current;
			
			newColor.a = m_overlayAlpha;
			GUI.color = newColor;
			GUI.Box(new Rect(m_dropdownRect.x, m_dropdownRect.y + m_dropdownRect.height, m_dropdownRect.width - 100, 100), "");
			
			
			GUILayout.BeginArea(new Rect(m_dropdownRect.x, m_dropdownRect.y + m_dropdownRect.height,700, 100));
			
			newColor.a = m_overlayTextAlpha;
			GUI.color = newColor;
			GUIStyle labelStyle = "Label";
			
			int currentIndex = 0;
			foreach(var currentString in m_availableStrings)
			{
				Rect newRect = GUILayoutUtility.GetRect(new GUIContent(currentString), labelStyle, GUILayout.MaxWidth(m_dropdownRect.width - 100));
				
				if(m_selectedIndex == currentIndex)
				{
					Color oldColor = newColor;
					newColor = new Color(0.0f, 0.0f, 0.4f, 0.4f);
					
					GUI.color = newColor;
					
					GUI.Box(newRect, "");
					GUI.color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a + 0.2f);
				}
				
				GUI.Label(newRect, currentString);	
				
				currentIndex++;
			}
			
			GUILayout.EndArea();
			GUI.color = current;
		}
	} 
	
	private static Rect m_dropdownRect;
	private static string m_lastString 				= string.Empty;
	private static string m_controlID 				= "dropdown";
	private static float m_overlayAlpha 			= 0.8f;
	private static float m_overlayTextAlpha 		= 0.5f;
	private static int m_selectedIndex 				= 0;
	private static List<string> m_availableStrings 	= new List<string>();
	private static bool m_showDropDown 				= false;
}
