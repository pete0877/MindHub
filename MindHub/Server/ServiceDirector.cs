using System;
using System.Collections;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Reflection;
using System.IO;
using Engine;
using Common;


namespace Server
{
	/// <summary>
	/// Class responsible for stopping / starting all managers / services.
	/// These responsibilities were not given to the Service.cs class in this project
	/// to keep the separation from Windows implementation of service.
	/// </summary>
	public class ServiceDirector
	{
		/// <summary>
		/// Config param: App plugins path.
		/// </summary>
		static public string SETTING_APPPLUGINSPATH		= "appPluginsPath";


		/// <summary>
		/// Config param: Port on which the main service should make itself available.
		/// </summary>
		static public string SETTING_SERVER_PORT				= "applicationServerPort";		

		/// <summary>
		/// Default server port
		/// </summary>
		static public int DEFAULT_SERVICE_PORT	= 8085;
		
		/// <summary>
		/// Service port on which service should reside (configured through CONFIG_SERVICEPORT)
		/// </summary>
		protected	int m_serverPort;

		/// <summary>
		/// Flag for if the server director has been initialized
		/// </summary>
		protected	bool m_initialized;

		/// <summary>
		/// Default constructor
		/// </summary>
		public ServiceDirector()
		{
			m_serverPort = 0;
			m_initialized = false;
		}


		/// <summary>
		/// Initilizes the configuration of the service director (e.g. find what port the main service should operate on)
		/// </summary>		
		public void Initialize()
		{
			SettingManager settingMgr = SettingManager.GetManager();
			settingMgr.SetLocation(Engine.Location.APP_SERVER);
			settingMgr.Initialize();

			m_serverPort = SmartInt.Parse(settingMgr[SETTING_SERVER_PORT]);
			if (m_serverPort == 0)
				m_serverPort = DEFAULT_SERVICE_PORT;

			m_initialized = true;
		}


		/// <summary>
		/// Starts services needed for the operator 
		/// </summary>
		/// <exception cref="ENotInitialized">If service director is not initialized</exception>
		public void StartServices()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			// Create remoting channel and register it:
			BinaryServerFormatterSinkProvider srv = new BinaryServerFormatterSinkProvider();
			TcpServerChannel chan = new TcpServerChannel(Def.ENGINE_NAME + "Channel", m_serverPort, srv);
			ChannelServices.RegisterChannel(chan);

			LogManager logMgr = null;
			try
			{
				// Initialize managers:
				logMgr = LogManager.GetManager();
				logMgr.SetFileName(Def.ENGINE_NAME + "App.log");
				logMgr.Initialize();

				EnvironmentManager environmentMgr = EnvironmentManager.GetManager();
				environmentMgr.Initialize();

				LockManager	lockMgr = LockManager.GetManager();
				lockMgr.Initialize();

				DBConnectionManager dbConnectionMgr = DBConnectionManager.GetManager();
				dbConnectionMgr.Initialize();

				SchemaManager schemaMgr = SchemaManager.GetManager();
				schemaMgr.Initialize();

				DataManager dataManager = DataManager.GetManager();
				dataManager.Initialize();

				ConfigManager configMgr = ConfigManager.GetManager();
				configMgr.Initialize();

				DataObjectCacheManager dataObjectCacheManager = DataObjectCacheManager.GetManager();
				dataObjectCacheManager.Initialize();

				StringManager stringMgr = StringManager.GetManager();
				stringMgr.Initialize();

				TemplateManager templateMgr = TemplateManager.GetManager();
				templateMgr.Initialize();

				// Register RemoteServerConnection for Client Object-Activation (COA):
				RemotingConfiguration.RegisterActivatedServiceType(typeof(ServerConnectionStub));
				LogManager.Log(this, "Remote Services Started");

				// Register code classes:
				DataObject.Register();
				UserDataObject.Register();
				CalendarDataObject.Register();
				TaskDataObject.Register();
				ProcessMapDataObject.Register();
				
				LoadPlugins();
				LogManager.Log(this, Engine.Def.ENGINE_NAME + " application service started");
			}
			catch(Exception e)
			{
				if (logMgr != null && logMgr.IsInitialized())
					LogManager.Log(this, "Server Startup Error: " + e.ToString());
				throw e;
			} 			
		}


		/// <summary>
		/// Stops services needed for the operator 
		/// </summary>
		/// <exception cref="ENotInitialized">If service director is not initialized</exception>
		public void StopServices()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			LogManager logMgr = null;
			try
			{
				TemplateManager templateMgr = TemplateManager.GetManager();
				templateMgr.UnInitialize();

				StringManager stringMgr = StringManager.GetManager();
				stringMgr.UnInitialize();

				ConfigManager configMgr = ConfigManager.GetManager();
				configMgr.UnInitialize();

				DataManager transManager = DataManager.GetManager();
				transManager.UnInitialize();

				SchemaManager schemaMgr = SchemaManager.GetManager();
				schemaMgr.UnInitialize();

				DBConnectionManager dbConnectionMgr = DBConnectionManager.GetManager();
				dbConnectionMgr.UnInitialize();

				LockManager	lockMgr = LockManager.GetManager();
				lockMgr.UnInitialize();

				EnvironmentManager environmentMgr = EnvironmentManager.GetManager();
				environmentMgr.UnInitialize();

				logMgr = LogManager.GetManager();
				LogManager.Log(this, Engine.Def.ENGINE_NAME + " application service stopped");
				logMgr.UnInitialize();
			}
			catch(Exception e)
			{
				if (logMgr != null && logMgr.IsInitialized())
					LogManager.Log(this, "Server Shutdown Error: " + e.ToString());

				throw e;
			} 
		}		

		/// <summary>
		/// Loads plugins.
		/// Attempts to read in every .dll file in the plugin path.
		/// Within each plugin "AppPlugin" type is searched for. 
		/// When found, "Load()" method is called.
		/// </summary>
		protected void LoadPlugins()
		{
			// Find where the app plugins are being loaded from:
			SettingManager settingMgr = SettingManager.GetManager();
			string pluginPath = settingMgr[SETTING_APPPLUGINSPATH];
			if (pluginPath == "")
				pluginPath = Directory.GetCurrentDirectory();

			// Get list of all .dll assemblies that could be loaded:
			string[] files = Directory.GetFiles(pluginPath, "*.dll");

			// Attempt to load all plugins:
			IEnumerator plugins = files.GetEnumerator();
			while (plugins.MoveNext())
			{				
				string fullPluginPath = (string) plugins.Current;
				Assembly assembly = Assembly.LoadFrom(fullPluginPath);
				if (assembly == null)
					continue;

				// Enusre this is a valid app plugin
				Type[] assemblyTypes = assembly.GetTypes();
				IEnumerator types = assemblyTypes.GetEnumerator();
				while (types.MoveNext())
				{
					Type pluginType = (Type) types.Current;
					if (pluginType.Namespace + ".AppPlugin" == pluginType.FullName)
					{
						// Find the loading function:
						MethodInfo loadMethod = pluginType.GetMethod("Load");
						if (loadMethod == null)
							continue;

						// Create instance of the plugin loader:
						Object obj = Activator.CreateInstance(pluginType);
						if (obj == null)
							continue;

						Object[] args = new Object[0];
						try
						{
							loadMethod.Invoke(obj, args);	
						}
						catch(Exception e)
						{
							LogManager.Log(this, "Assembly could not be loaded: " + fullPluginPath + " (" + e.ToString() + ")", LogManager.Level.ERROR);			
							continue;
						}
						LogManager.Log(this, "Assembly loaded: " + fullPluginPath, LogManager.Level.NORMAL);

						break;						
					}
				}
			}
		}
	}
}
