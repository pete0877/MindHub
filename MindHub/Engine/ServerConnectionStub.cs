using System;
using System.Runtime.Remoting;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Engine
{
	/// <summary>
	/// Summary description for RemoteServerConnection.
	/// </summary>
	public class ServerConnectionStub : MarshalByRefObject
	{
		/// <summary>
		/// ID of this connection
		/// </summary>
		protected string m_ID;

		/// <summary>
		/// Transaction object attached from the web server process:
		/// </summary>
		protected Transaction m_transaction;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="ID">Server connection ID</param>
		public ServerConnectionStub(string ID)
		{
			m_ID = ID;
		}


		/// <summary>
		/// Returns connection ID assigned to this connection.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Unique conneciton ID</returns>
		public string GetID()
		{
			return m_ID;
		}


		/// <summary>
		/// Returns self representation as string - used in error handling
		/// </summary>
		/// <returns>Self as string</returns>
		override public string ToString()
		{
			return "ServerConnectionStub ID: " + GetID();
		}	


		/// <summary>
		/// Disposes the attached transaction (on the applicaiton side). This in effect causes the database connection to be freed up back into the pool (see DB connection manager)
		/// </summary>
		public void DisposeTransaction()
		{
			if (m_transaction != null)
			{
				m_transaction.Dispose();
				m_transaction = null;
			}
		}


		/// <summary>
		/// Receives call from ServerConnectionProxy::Send()
		/// </summary>
		/// <param name="environmentID">Environment ID (possibly null)</param>
		/// <param name="classID">Class ID of the receiver (registered manager which should receive this call)</param>
		/// <param name="method">Method name to be invoked</param>
		/// <param name="args">List of arguments (possibly null). If not null, it should be of size at least 1 and have the [0] cell not set.</param>
		/// <returns>Object returned by the call to the specified</returns>
		public Object Receive(string environmentID, string classID, string method, Object[] args)
		{			
			try
			{
				Object result = null;

				if (m_transaction == null && environmentID != null)
					m_transaction = new Transaction(environmentID);

				Object manager = Manager.GetManager(classID);
				if (manager == null)
					throw new EInvalidParameter(this, "classID", classID, "Manager singelton not registered.");
		
				if (args != null)
					args[0] = m_transaction;

				Type t = manager.GetType();

				result = t.InvokeMember(method, BindingFlags.InvokeMethod, null, manager, args);
				return result;
			}
			catch (Exception e)
			{
				LogManager.Log(this, "Method " + classID + "::" + method + " failed: " + e.ToString(), LogManager.Level.ERROR);
				throw new Exception("Method " + classID + "::" + method + " failed.", e);			
			}			
		}


		/// <summary>
		/// Initializes the life-time of the object to infinite.
		/// </summary>
		/// <returns>Null</returns>
		public override Object InitializeLifetimeService()
		{
			return null;
		}  

	}
}
