///////////////////////////////////////////////////////////
// 
// LevelNetworkCommandRequest.cs
//
// What it does: Represents the transaction of requesting a command and tracks the progress of the command's execution.
//
// Notes:
// 
// To-do: Command cancelling.
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;

public class LevelNetworkCommandRequest 
{
	public enum CommandState
	{
		Active,
		Completed
	}
	
	public LevelNetworkCommandRequest(NodeSession session, LevelNetworkCommand command)
	{
		m_command = command;
		m_session = session;
	}
	
	public void UpdateProgress(float deltaProgress)
	{
		m_progress += deltaProgress;
		
		if(m_progress > m_command.ExecutionTime)
		{
			m_state = CommandState.Completed;	
		}
	}
	
	public CommandState State
	{
		get { return m_state; }	
	}
	
	public LevelNetworkCommand Command
	{
		get { return m_command; }	
	}
	
	public NodeSession Session
	{
		get { return m_session; }	
	}
	
	public float Progress
	{
		get { return m_progress / m_command.ExecutionTime; }	
	}
	
	private float 				m_progress 	= 0.0f;
	private CommandState 		m_state 	= CommandState.Active;
	private LevelNetworkCommand m_command 	= null;
	private NodeSession			m_session	= null;
}