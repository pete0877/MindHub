using System;
using System.IO;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Represents connection between web server and application server
	/// </summary>
	public class ServerConnectionProxy : IDisposable
	{
		/// <summary>
		/// Connection state
		/// </summary>
		public enum State { Used, Free }

 		/// <summary>
		/// ID of this connection
		/// </summary>
		protected string m_ID;

		/// <summary>
		/// Usage state (if it is currently used by any thread)
		/// </summary>
		protected State m_state;

		/// <summary>
		/// Reference to remote instance of the connection class (created on application server)
		/// </summary>
		protected ServerConnectionStub m_serverConnectionStub;
	
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="ID">Server connection ID</param>
		/// <param name="host">Server host name</param>
		/// <param name="port">Server port number</param>
		public ServerConnectionProxy(string ID)
		{
			m_ID = ID;

			// Create instance of the remote connection:
			m_serverConnectionStub = new ServerConnectionStub(ID);
		}


		/// <summary>
		/// Disposes self by freeing up its status (using server connection manager)
		/// </summary>
		public void Dispose()
		{
			ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
			serverConnectionMgr.ReleaseConnection(this);
		}


		/// <summary>
		/// Returns self representation as string - used in error handling
		/// </summary>
		/// <returns>Self as string</returns>
		override public string ToString()
		{
			return "ServerConnectionProxy ID: " + GetID();
		}	


		/// <summary>
		/// Returns connection ID assigned to this connection.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns>Unique conneciton ID</returns>
		public string GetID()
		{
			return m_ID;
		}


		/// <summary>
		/// Sets current connection usage state.
		/// </summary>
		/// <param name="state">State</param>
		public void SetState(State state)
		{
			if (m_state == State.Used && state == State.Free && m_serverConnectionStub != null)
				m_serverConnectionStub.DisposeTransaction();

			m_state = state;
		}


		/// <summary>
		/// Returns current conneciton usage state.
		/// Performs get by value (safe)
		/// </summary>
		/// <returns></returns>
		public State GetState()
		{
			return m_state;
		}


		/// <summary>
		/// Invokes a method to specified receiver singelton via remoting. Environment ID is optional (might be used depending on if call is made to a manager to perform specific action within an environment or not)
		/// </summary>
		/// <param name="environmentID">Environment ID (can be null)</param>
		/// <param name="receiver">Receiver (e.g. "Engine.DataManager")</param>
		/// <param name="method">Method being invoked (e.g. "CommitTransaction")</param>
		/// <param name="args">List of arguments (possibly null). If not null, it should be of size at least 1 and have the [0] cell not set.</param>
		/// <returns></returns>
		public Object Send(string environmentID, string receiver, string method, Object[] args)
		{
			Object result = m_serverConnectionStub.Receive(environmentID, receiver, method, args);
			return result;
		}
	}
}
