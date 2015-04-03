using System;
using System.Threading;
using System.Collections;
using Engine;

namespace WebAPI
{
	// todo: SessionManager needs to save sessions to local file (or something of that sort) in order to be able to restored them upon fail or kill-restart.
	public class SessionManager : Manager
	{
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected SessionManager g_manager;

		/// <summary>
		/// List of sessions for each environment (where environment ID is hash key)
		/// </summary>
		protected Hashtable m_enviromentSessions;

		/// <summary>
		/// List of reader-writer locks on m_enviromentSessions for each environment (where environment ID is hash key)
		/// </summary>
		protected Hashtable m_enviromentLocks;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static SessionManager()
		{
			g_manager = new SessionManager();
		}


		/// <summary>
		/// Default constructor. Creates empty session and lock list.
		/// </summary>
		protected SessionManager()
		{			
			m_enviromentSessions = new Hashtable();
			m_enviromentLocks = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static SessionManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes the empty lists of sessions and locks
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			EnvironmentManager envMgr = EnvironmentManager.GetManager();

			// For each environment initialize the sessions set:
			IDictionaryEnumerator itr = envMgr.GetEnvironments();
			while (itr.MoveNext())
			{
				Engine.Environment environment = (Engine.Environment) itr.Value;
				string environmentID = environment.ID;

				// Create new sessions list hash for this environment:
				Hashtable sessionsList = new Hashtable();
				m_enviromentSessions[environmentID] = sessionsList;
				m_enviromentLocks[environmentID] = new ReaderWriterLock();
			}

			m_initialized = true;
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns session referenced by the given session ID in given environment.
		/// If session is not found, null is returned.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <param name="sessionID">Session ID</param>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>Session object reference</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters.</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public Session GetSession(string sessionID, string environmentID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (sessionID == null)
				throw new EInvalidParameter(this, "sessionID");

			// Find sessions hash for referenced environment
			Hashtable sessionsList = (Hashtable) m_enviromentSessions[environmentID];
			ReaderWriterLock sessionsListLock = (ReaderWriterLock) m_enviromentLocks[environmentID];

			if (sessionsList == null || sessionsListLock == null)
				throw new EInvalidParameter(this, "environmentID", environmentID, "Provided environment ID does not exist or there is no sessions list assiciated with it");

			// Get the session from the hash:
			sessionsListLock.AcquireReaderLock(Int32.MaxValue);
			Engine.Session session = (Engine.Session) sessionsList[sessionID];
			sessionsListLock.ReleaseReaderLock();
			
			return session;
		}


		/// <summary>
		/// Adds session to the active sessions lists.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="session">Valid session</param>
		/// <exception cref="EInvalidParameter">If parameter is null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public void AddSession(Engine.Session session)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (session == null)
				throw new EInvalidParameter(this, "session");

			string environmentID = session.EnvironmentID;

			// Find sessions hash for referenced environment
			Hashtable sessionsList = (Hashtable) m_enviromentSessions[environmentID];
			ReaderWriterLock sessionsListLock = (ReaderWriterLock) m_enviromentLocks[environmentID];

			if (sessionsList == null || sessionsListLock == null)
				throw new EInvalidParameter(this, "environmentID", environmentID, "Provided environment ID does not exist or there is no sessions list assiciated with it");

			// Add the session to the sessions list:
			sessionsListLock.AcquireWriterLock(Int32.MaxValue);
			sessionsList[session.ID] = session;
			sessionsListLock.ReleaseWriterLock();
		}


		/// <summary>
		/// Removes given session from active sessions list
		/// </summary>
		/// <param name="session">Valid session to be removed</param>
		/// <exception cref="EInvalidParameter">If parameter is null.</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public void RemoveSession(Engine.Session session)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (session == null)
				throw new EInvalidParameter(this, "session");

			string environmentID = session.EnvironmentID;

			// Find sessions hash for referenced environment
			Hashtable sessionsList = (Hashtable) m_enviromentSessions[environmentID];
			ReaderWriterLock sessionsListLock = (ReaderWriterLock) m_enviromentLocks[environmentID];

			if (sessionsList == null || sessionsListLock == null)
				throw new EInvalidParameter(this, "environmentID", environmentID, "Provided environment ID does not exist or there is no sessions list assiciated with it");
			
			// remove the session from the sessions list:
			sessionsListLock.AcquireWriterLock(Int32.MaxValue);
			sessionsList[session.ID] = null;
			sessionsListLock.ReleaseWriterLock();
		}
	}
}


