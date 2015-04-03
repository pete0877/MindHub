using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Engine
{
	/// <summary>
	/// Manages sets of configurations for environments.
	/// </summary>
	public class ConfigManager : Manager
	{
		/// <summary>
		/// Name of the table storing the configurations
		/// </summary>
		static public string SCHEMA_CONFIG		= "tconfiguration";

		/// <summary>
		/// Primary key in configuration table
		/// </summary>
		static public string COLUMN_CONFIGID	= "configid";

		/// <summary>
		/// Column storing the XML representation of the configuration
		/// </summary>
		static public string COLUMN_CONFIGXML	= "configxml";

		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected ConfigManager g_manager;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 		

		/// <summary>
		/// Hash of configuration sets keyed by environment IDs
		/// </summary>
		protected Hashtable m_enviromentConfigs;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static ConfigManager()
		{
			g_manager = new ConfigManager();
		}

		/// <summary>
		/// Default constructor. Creates empty configurations list.
		/// </summary>
		protected ConfigManager()
		{			
			m_enviromentConfigs = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static ConfigManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes the cached list of configurations
		/// </summary>
		public override void Initialize()
		{
			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			m_location = envMgr.GetLocation();

			// If the local environment is on app server, get the configurations
			// from database. Else get all configurations from remote call:
			if (m_location == Location.APP_SERVER)
			{	
				// For each environment initialize the configuration set:
				IDictionaryEnumerator itr = envMgr.GetEnvironments();
				while (itr.MoveNext())
				{
					Environment environment = (Environment) itr.Value;
					string environmentID = environment.ID;

					using (Transaction transaction = new Transaction(environmentID))
					{
						// Create new config hash for this environment:
						Hashtable configs = new Hashtable();
						m_enviromentConfigs[environmentID] = configs;					

						// Read in the configuration table:
						DataReader reader = new DataReader(SCHEMA_CONFIG);
						reader += COLUMN_CONFIGID;
						reader += COLUMN_CONFIGXML;
						reader.Where(COLUMN_CONFIGID + " is not null");
						for (reader.Load(transaction); reader.Read();)
						{
							// For each configuration, read in config ID and configuration XML
							Config config = new Config();
							string configID = reader[COLUMN_CONFIGID];
							config.Initialize(configID, reader[COLUMN_CONFIGXML]);
							configs.Add(configID, config);
						}
					}
				}
			}
			else		
			{
				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				ServerConnectionProxy proxy = serverConnectionMgr.GetServerConnection();
				m_enviromentConfigs = (Hashtable) proxy.Send(null, GetType().ToString(), "GetStateRemote", new Object[1]);
			}

			base.Initialize();
			LogManager.Log(this, "Initialized");
		}

		/// <summary>
		/// Returns state of the manager 
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <returns>Current state</returns>
		public Hashtable GetStateRemote(Transaction transaction)
		{
			return m_enviromentConfigs;
		}

		/// <summary>
		/// Returns configuration referenced by the given configuration ID in given environment.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="configID">Configuration ID</param>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>Configuration object reference</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters are null or the configuration is not to be found</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public Config GetConfig(string configID, string environmentID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (configID == null)
				throw new EInvalidParameter(this, "configID");

			// Find configs hash for referenced environment
			Hashtable configs = (Hashtable) m_enviromentConfigs[environmentID];
			if (configs == null)
				throw new EInvalidParameter(this, "environmentID", environmentID, "Provided environment ID does not exist or there is no configuration list assiciated with it");

			// Find right config object and return reference to it:
			Config config = (Config) configs[configID];
			if (config == null)
				throw new EInvalidParameter(this, "configID", configID, "Configuration ID not found");
			
			return config;
		}


		/// <summary>
		/// Returns configuration referenced by the given configuration ID in given environment (referenced by transaction).
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="configID">Configuration ID</param>
		/// <param name="transaction">Transaction</param>
		/// <returns>Configuration object reference</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters are null or the configuration is not to be found</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public Config GetConfig(string configID, Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			return GetConfig(configID, transaction.EnvironmentID);
		}


		/// <summary>
		/// Causes the manager to refresh all configuration sets (for all environments)
		/// </summary>
		public void Refresh()
		{
			// In order for the web-side config manager to refresh, the app-side manager has to refresh first.
			m_enviromentConfigs = new Hashtable();
			m_initialized = false;
			Initialize();
		}
	}
}
