using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameFlowWrapper))] 
public class GameFlowWrapperEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		GameFlowWrapper wrapper = (GameFlowWrapper)target;
		
		wrapper.View = (WorldView)EditorGUILayout.EnumPopup(wrapper.View);
	}
}
