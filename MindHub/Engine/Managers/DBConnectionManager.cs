using System;
using System.Collections;
using System.Threading;
using Common;

namespace Engine
{
	/// <summary>
	/// Database connection manager
	/// </summary>
	public class DBConnectionManager : Manager
	{
		/// <summary>
		/// Configuration parameter for number of db connections that should be places in the pool  (for each environment)
		/// </summary>
		static public string SETTING_CONNECTIONPOOL	= "dbConnectionPool";		

		/// <summary>
		/// Number of server connections pooled and distributed to transactions
		/// </summary>
		public static int DEFAULT_CONNECTIONS_POOL = 20;

		/// <summary>
		/// Number of miliseconds that the thread will sleep for if the DB connection pool is depleted, before attempting to grab a connection again
		/// </summary>
		public static int DEFAULT_LOCK_WAIT_TIME = 500;

		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected DBConnectionManager g_manager;

		/// <summary>
		/// Number of connections to pool (per environment)
		/// </summary>
		protected int m_connectionPoolSize;

		/// <summary>
		/// Hashtable of server connections
		/// </summary>
		protected Hashtable m_envConnections;
		
		/// <summary>
		/// Queue of open (free) connections
		/// </summary>
		protected Hashtable m_envFreeConnections;

		/// <summary>
		/// Synch lock on the m_freeConnections operations
		/// </summary>
		protected Hashtable m_envLocks;


		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static DBConnectionManager()
		{
			g_manager = new DBConnectionManager();
		}


		/// <summary>
		/// Default contructor
		/// </summary>
		protected DBConnectionManager()
		{
			// Create the top-level hashes
			m_envConnections = new Hashtable();
			m_envFreeConnections = new Hashtable();
			m_envLocks = new Hashtable();
		}


		/// <summary>
		/// Initializes instance of the manager:
		/// </summary>
		public override void Initialize()
		{	
			// Make sure we are on application side:
			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			if (envMgr.GetLocation() != Location.APP_SERVER)
				throw new Exception("DB Connection Manager cannot be initialized anywhere but in the application tier");

			// Determine connecciton pool size:
			SettingManager settingMgr = SettingManager.GetManager();
 			m_connectionPoolSize = SmartInt.Parse(settingMgr[SETTING_CONNECTIONPOOL]);
			if (m_connectionPoolSize == 0)
				m_connectionPoolSize = DEFAULT_CONNECTIONS_POOL;

			// For each environment create default number of connections
			// (populate m_envConnections, m_envFreeConnections and m_envLocks with appropriate instances)
			IDictionaryEnumerator itr = envMgr.GetEnvironments();
			while (itr.MoveNext())
			{
				Environment environment = (Environment) itr.Value;

				// Each environment has hash of all known db connections:
				Hashtable connections = new Hashtable();
				m_envConnections[environment.ID] = connections;

				// And each environment has a queue of all free db connections:
				Queue connectionsQueue = new Queue();
				m_envFreeConnections[environment.ID] = connectionsQueue;

				// And each environment has a lock on the connecitons list:
				ReaderWriterLock readerWriterLock = new ReaderWriterLock();
				m_envLocks[environment.ID] = readerWriterLock;

				// Create connections:
				for (int n = 0; n < m_connectionPoolSize; n++)
				{
					string ID = n.ToString();
					DBConnection connection = new DBConnection(ID, environment);
					connections[ID] = connection;
					connectionsQueue.Enqueue(connection);
				}	
			}

			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static DBConnectionManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Returns DB connection instance. This db connection can be returned to the connection manager by calling DBConnection.Dispose()
		/// If the connection pool has been depleted, this call waits and does not return until next available connection is available.
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>New instance of DB connection for given environment</returns>
		/// <exception cref="EInvalidParameter">If environment ID is null environment cannot be found.</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public DBConnection GetDBConnection(string environmentID)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			// Get the lock:
			ReaderWriterLock readerWriterLock = (ReaderWriterLock) m_envLocks[environmentID];
			if (readerWriterLock == null)
				throw new EInvalidParameter(this, "environmentID");

			// Get the queue:
			Queue connectionsQueue = (Queue) m_envFreeConnections[environmentID];
			if (connectionsQueue == null)
				throw new EInvalidParameter(this, "environmentID");

			// Wait till at least one connection is open:
			readerWriterLock.AcquireWriterLock(Int32.MaxValue);
			while (connectionsQueue.Count == 0)
			{
				readerWriterLock.ReleaseWriterLock();
				Thread.Sleep(DEFAULT_LOCK_WAIT_TIME);
				readerWriterLock.AcquireWriterLock(Int32.MaxValue);
			}
			
			// Now we have the writer lock on and we know that there is at least one
			// connection in the queue:
			DBConnection dbConnection = (DBConnection) connectionsQueue.Dequeue();
			readerWriterLock.ReleaseWriterLock();

			dbConnection.SetState(DBConnection.State.Used);

			return dbConnection;
		}


		/// <summary>
		/// Releases given connection and puts it back in the queue. This method should not be called by outside connection-using clients. Instead, call to DBConnection.Dispose() should be made (which in turn calls this method )
		/// </summary>
		/// <param name="connection">DB connection</param>
		/// <exception cref="ENotInitialized">If instance of this manger was not initialized yet.</exception>
		/// <exception cref="EInvalidParameter">If connection parameter is null, if the connection is not on manager's list of used connection</exception>
		public void ReleaseConnection(DBConnection dbConnection)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (dbConnection == null)
				throw new EInvalidParameter(this, "dbConnection");

			if (dbConnection.GetState() != DBConnection.State.Used)
				throw new EInvalidParameter(this, "dbConnection");

			// Free up the connection and put it the queue:
			dbConnection.SetState(DBConnection.State.Free);

			// Get the lock:
			ReaderWriterLock readerWriterLock = (ReaderWriterLock) m_envLocks[dbConnection.EnvironmentID];
			if (readerWriterLock == null)
				throw new EInvalidParameter(this, "dbConnection.EnvironmentID");

			// Get the queue:
			Queue connectionsQueue = (Queue) m_envFreeConnections[dbConnection.EnvironmentID];
			if (connectionsQueue == null)
				throw new EInvalidParameter(this, "dbConnection.EnvironmentID");

			// Wait till at least one connection is open:
			readerWriterLock.AcquireWriterLock(Int32.MaxValue);
			connectionsQueue.Enqueue(dbConnection);
			readerWriterLock.ReleaseWriterLock();
		}
	}
}
 