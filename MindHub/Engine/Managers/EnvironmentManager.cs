using System;
using System.Collections;
using System.Runtime.Remoting;

namespace Engine
{
	/// <summary>
	/// Environment manager. 
	/// Keeps track of various applications that might be running on the same server).
	/// This manager is also aware of it's tier location (web or application) and provides this
	/// information to other components of the system.
	/// </summary>
	public class EnvironmentManager : Manager
	{
		/// <summary>
		/// Configuration file section for list of environments being initialized
		/// </summary>
		static public string SETTING_ENVIRONMENTS	= "environments";

		/// <summary>
		/// Configuration file section for database type
		/// </summary>
		static public string SETTING_TYPE			= "type";

		/// <summary>
		/// Configuration file section for database connection string
		/// </summary>
		static public string SETTING_CONNECT		= "connect";

		/// <summary>
		/// Configuration file section for database type
		/// </summary>
		static public string SETTING_TYPE_SQL		= "SQL";

		/// <summary>
		/// Configuration file section for environment UI folder (OS path)
		/// </summary>
		static public string SETTING_UIPATHOS		= "uipathos";

		/// <summary>
		/// Configuration file section for environment UI folder (Web path)
		/// </summary>
		static public string SETTING_UIPATHWEB		= "uipathweb";
	
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected EnvironmentManager g_manager;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 	

		/// <summary>
		/// Dictionary of environments
		/// </summary>
		protected Hashtable m_environments;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static EnvironmentManager()
		{
			g_manager = new EnvironmentManager();
		}


		/// <summary>
		/// Instance contructor.
		/// </summary>
		protected EnvironmentManager()
		{
			m_environments = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static EnvironmentManager GetManager()
		{			
			return g_manager;
		}

	
		/// <summary>
		/// Returns the location with which this instance was initialized with.
		/// </summary> 
		/// <returns>Location (e.g. Location.APP_SERVER)</returns>
		public Location GetLocation()
		{			
			return m_location;
		}


		/// <summary>
		/// Initializes instance of this manager at specific location.
		/// </summary>
		/// <param name="location">Execution location</param>
		public override void Initialize()
		{
			// Depending on what the location is we either load environments from configuraration
			// or we might load environments by making remote calls to the environment
			// manager on the Server side and grabbing all environments from there.

			SettingManager settingMgr = SettingManager.GetManager();
			m_location = settingMgr.GetLocation();

			if (m_location == Location.APP_SERVER)
			{	
				// Get list of environments to initialize (it's a comma-delimited list)
				string environmentIDsList = settingMgr[SETTING_ENVIRONMENTS];
				if (environmentIDsList == null)
					throw new Exception("Application server cofiguration file misformatted (e.g. missing section: " + SETTING_ENVIRONMENTS + ")");

				// For each environment, initialize Environment instance and register it:
				string[] environmentIDs = environmentIDsList.Split(',');
				IEnumerator itr = environmentIDs.GetEnumerator();
				while (itr.MoveNext())
				{
					DBConnection.DBType dbType;
					string environmentID = (string) itr.Current;

					IDictionary environmentSettings = settingMgr.GetSection(environmentID);
					if (environmentSettings == null)
						throw new Exception("Environment configuration section missing or misformatted: " + environmentID);

					// Determine DB type:
					string typeString = (string) environmentSettings[SETTING_TYPE];
					if (typeString == SETTING_TYPE_SQL)
						dbType = DBConnection.DBType.SQLServer;
					else
						throw new Exception("Environment configuration item (" + SETTING_TYPE + ") is missing, misformatted, empty or unsupported");

					string connect = (string) environmentSettings[SETTING_CONNECT];
					if (connect == null || connect.Length == 0)
						throw new Exception("Environment configuration item (" + SETTING_CONNECT + ") is missing, misformatted or empty");

					string uiPathOS = (string) environmentSettings[SETTING_UIPATHOS];
					if (uiPathOS == null || uiPathOS.Length == 0)
						throw new Exception("Environment configuration item (" + SETTING_UIPATHOS + ") is missing, misformatted or empty");

					string uiPathWeb = (string) environmentSettings[SETTING_UIPATHWEB];
					if (uiPathWeb == null || uiPathWeb.Length == 0)
						throw new Exception("Environment configuration item (" + SETTING_UIPATHWEB + ") is missing, misformatted or empty");

					Environment env = new Environment(environmentID, dbType, connect, uiPathOS, uiPathWeb);
					m_environments[environmentID] = env;
				}
			}
			else		
			{
				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				ServerConnectionProxy proxy = serverConnectionMgr.GetServerConnection();
				m_environments = (Hashtable) proxy.Send(null, GetType().ToString(), "GetStateRemote", new Object[1]);
			}
 
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns state of the manager
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <returns>Current state</returns>
		public Hashtable GetStateRemote(Transaction transaction)
		{
			return m_environments;
		}


		/// <summary>
		/// Returns enumeration of environments.
		/// Members of enumeration are not copies of registered classes, hence this call is not safe.
		/// </summary> 
		/// <returns>Environment dictionary enumerator</returns>
		/// <exception cref="ENotInitialized">If manager is not initialized.</exception>
		public IDictionaryEnumerator GetEnvironments()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			return m_environments.GetEnumerator();
		}

		
		/// <summary>
		/// Returns environment of given ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
 		/// <returns>Environment</returns>
		/// <exception cref="EInvalidParameter">If the environment ID is null or invalid</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public Environment GetEnvironment(string environmentID)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			Environment environment = (Environment) m_environments[environmentID]; 
			if (environment == null)
				throw new EInvalidParameter(this, "environmentID", environmentID, "Provided environment ID does not exist");

			return environment;
		}
	}
}
 