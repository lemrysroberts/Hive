using UnityEngine;
using System;
using System.Collections;

[AttributeUsage(AttributeTargets.All)]
public class NodeFunction : Attribute 
{
	public string NodeFunctionName
	{
		get { return m_functionName; }	
	}
	
	public NodeFunction(string functionName)
	{
		m_functionName = functionName;	
	}
	
	private string m_functionName;
}
