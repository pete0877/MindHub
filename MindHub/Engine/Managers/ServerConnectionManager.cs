using System;
using System.Collections;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Configuration;
using Common;

namespace Engine
{
	/// <summary>
	/// Application server connection manager. This class is to be used in components other then the application server (E.g use it on the web sever).
	/// </summary>
	public class ServerConnectionManager : Manager
	{
		/// <summary>
		/// Default server port
		/// </summary>
		static public int DEFAULT_SERVICE_PORT	= 8085;

		/// <summary>
		/// Configuration parameter for server host name
		/// </summary>
		static public string SETTING_APPSERVERHOST		= "applicationServerHost";

		/// <summary>
		/// Configuration parameter for server post number
		/// </summary>
		static public string SETTING_APPSERVERPORT		= "applicationServerPort";		

		/// <summary>
		/// Configuration parameter for number of server connections that should be places in the pool 
		/// </summary>
		static public string SETTING_CONNECTIONPOOL	= "serverConnectionPool";		

		/// <summary>
		/// Default number of server connections that should be places in the pool 
		/// </summary>
		public static int DEFAULT_CONNECTIONS_POOL = 20;

		/// <summary>
		/// Number of miliseconds that the thread will sleep for if the app-server connection pool is depleted, before attempting to grab a connection again
		/// </summary>
		public static int DEFAULT_LOCK_WAIT_TIME = 500;

		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected ServerConnectionManager g_manager;

		/// <summary>
		/// Name of the machine hosting the application server 
		/// </summary>
		protected string m_appServerHost;

		/// <summary>
		/// Port number hosting the application server 
		/// </summary>
		protected int m_appServerPort;

		/// <summary>
		/// Number of connections to pool
		/// </summary>
		protected int m_connectionPoolSize;

		/// <summary>
		/// Hashtable of server connections
		/// </summary>
		protected Hashtable m_connections;
		
		/// <summary>
		/// Queue of open (free) connections
		/// </summary>
		protected Queue m_freeConnections;

		/// <summary>
		/// Synch lock on the m_freeConnections operations
		/// </summary>
		protected ReaderWriterLock m_lock;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static ServerConnectionManager()
		{
			g_manager = new ServerConnectionManager();
		}		
		

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ServerConnectionManager()
		{
			m_connections = new Hashtable();
			m_freeConnections = new Queue();
			m_lock = new ReaderWriterLock();
		}


		/// <summary>
		/// Initializes instance of lock manager:
		/// </summary>
		public override void Initialize()
		{
			// Get the cofiguration:
			SettingManager settingMgr = SettingManager.GetManager();
			m_appServerHost = (string) settingMgr[SETTING_APPSERVERHOST];

			m_appServerPort = SmartInt.Parse(settingMgr[SETTING_APPSERVERPORT]);
			if (m_appServerPort == 0)
				m_appServerPort = DEFAULT_SERVICE_PORT;

			m_connectionPoolSize = SmartInt.Parse(settingMgr[SETTING_CONNECTIONPOOL]);
			if (m_connectionPoolSize == 0)
				m_connectionPoolSize = DEFAULT_CONNECTIONS_POOL;

			string connectionString = ComposeConnectionString(m_appServerHost, m_appServerPort);
			RemotingConfiguration.RegisterActivatedClientType(typeof(ServerConnectionStub), connectionString);

			// Initialize remote connection to the server connection manager on the remote server process:
			for (int n = 0; n < m_connectionPoolSize; n++)
			{
				string ID = n.ToString();
				ServerConnectionProxy connection = new ServerConnectionProxy(ID);
				m_connections[ID] = connection;
				m_freeConnections.Enqueue(connection);
			}
			
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static ServerConnectionManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Returns name of the host where the application server is located
		/// Performs return by value (safe)
		/// </summary>
		/// <returns>Machine name hosting the service</returns>
		public string GetServerHost()
		{
			return string.Copy(m_appServerHost);
		}


		/// <summary>
		/// Returns port of the host where the application server is located
		/// Performs return by value (safe)
		/// </summary>
		/// <returns>Port number hosting the service</returns>
		public int GetServerPort()
		{
			return m_appServerPort;
		}


		/// <summary>
		/// Returns app-server connection instance. This app-server connection can be returned to the connection manager by calling ServerConnection.Dispose()
		/// If the connection pool has been depleted, this call waits and does not return until next available connection is available.
		/// </summary>
		/// <returns>Server connection assigned to this request</returns>
		/// <exception cref="ENotInitialized">If instance of this manger was not initialized yet.</exception>
		public ServerConnectionProxy GetServerConnection()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			// Wait till at least one connection is open:
			m_lock.AcquireWriterLock(Int32.MaxValue);
			while (m_freeConnections.Count == 0)
			{
				m_lock.ReleaseWriterLock();
				Thread.Sleep(DEFAULT_LOCK_WAIT_TIME);
				m_lock.AcquireWriterLock(Int32.MaxValue);
			}

			// Now we have the writer lock on and we know that there is at least one
			// connection in the queue:
			ServerConnectionProxy connectionProxy = (ServerConnectionProxy) m_freeConnections.Dequeue();
			m_lock.ReleaseWriterLock();

			connectionProxy.SetState(ServerConnectionProxy.State.Used);
			return connectionProxy;
		}


		/// <summary>
		/// Releases given connection and puts it back in the queue. This method should not be called by outside connection-using clients. Instead, call to ServerConnection.Dispose() should be made (which in turn calls this method )
		/// </summary>
		/// <param name="connection">Server connection</param>
		/// <exception cref="ENotInitialized">If instance of this manger was not initialized yet.</exception>
		/// <exception cref="EInvalidParameter">If connection parameter is null, if the connection is not on manager's list of used connection</exception>
		public void ReleaseConnection(ServerConnectionProxy connectionProxy)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (connectionProxy == null)
				throw new EInvalidParameter(this, "connectionProxy");

			if (connectionProxy.GetState() != ServerConnectionProxy.State.Used)
				throw new EInvalidParameter(this, "connectionProxy");

			// Free up the connection and put it the queue:
			connectionProxy.SetState(ServerConnectionProxy.State.Free);
			m_lock.AcquireWriterLock(Int32.MaxValue);
			m_freeConnections.Enqueue(connectionProxy);
			m_lock.ReleaseWriterLock();
		}


		/// <summary>
		/// Connects to remote server objects and returns reference to it.
		/// </summary>
		/// <param name="type">Object type</param>
		/// <param name="className">Class name</param>
		/// <returns>Reference to remote object</returns>
		public object GetSingeltonObject(Type type, string className) 
		{
			string uri = ComposeClassConnectionString(m_appServerHost, m_appServerPort, className);
			try 
			{				
				object remoteObject = Activator.GetObject(type, uri);
				if (remoteObject == null)
					throw new Exception("Could not create reference to remote object: " + uri);
					
				return remoteObject;		
			}
			catch (Exception e)
			{
				throw new Exception("Could not create reference to remote object: " + uri + ": " + e);
			}
		}


		/// <summary>
		/// Composes TCP remoting URI connection string given host, port 
		/// </summary>
		/// <param name="host">Host name (e.g. "localhost")</param>
		/// <param name="port">Port number (e.g. "8085")</param>
		/// <returns>URI connection string (e.g. tcp://localhost:8085)</returns>
		public static string ComposeConnectionString(string host, int port)
		{
			return "tcp://" + host + ":" + port.ToString();
		}


		/// <summary>
		/// Composes TCP remoting URI connection string given host, port and class name of the remoting object
		/// </summary>
		/// <param name="host">Host name (e.g. "localhost")</param>
		/// <param name="port">Port number (e.g. "8085")</param>
		/// <param name="remoteClass">Remote's object class name (e.g. "EnvironmentManager")</param>
		/// <returns>URI connection string (e.g. tcp://localhost:8085/MindHubEnvironmentManager)</returns>
		public static string ComposeClassConnectionString(string host, int port, string remoteClass)
		{
			return "tcp://" + host + ":" + port.ToString() + "/" + Def.ENGINE_NAME + remoteClass;
		}
	}
}
 
 
