using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Reflection;
using System.IO;
using Engine;

namespace WebAPI 
{
	/// <summary>
	/// ASP.Net Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Config param: Web plugins path.
		/// </summary>
		static public string SETTING_WEBPLUGINSPATH		= "webPluginsPath";

		/// <summary>
		/// Overriden method used to identify instances of this class. Used mostly during error handling.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "WebAPI";
		}

		/// <summary>
		/// Application contructor
		/// </summary>
		public Global()
		{
			InitializeComponent();
		}	
		

		/// <summary>
		/// Application start handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Application_Start(Object sender, EventArgs e)
		{
			// Create remote channel
			BinaryClientFormatterSinkProvider clnt = new BinaryClientFormatterSinkProvider();
			TcpClientChannel chan = new TcpClientChannel(Def.ENGINE_NAME + "Channel", clnt);
			ChannelServices.RegisterChannel(chan);

			LogManager logMgr = null;

			try
			{
				// Initialize managers:
				SettingManager settingMgr = SettingManager.GetManager();
				settingMgr.SetLocation(Engine.Location.WEB_SERVER);
				settingMgr.Initialize();

				logMgr = LogManager.GetManager();
				logMgr.SetFileName(Def.ENGINE_NAME + "Web.log");
				logMgr.Initialize();

				LockManager	lockMgr = LockManager.GetManager();
				lockMgr.Initialize();

				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				serverConnectionMgr.Initialize();

				EnvironmentManager environmentMgr = EnvironmentManager.GetManager();
				environmentMgr.Initialize();

				SecurityManager securityMgr = SecurityManager.GetManager();
				securityMgr.Initialize();

				SchemaManager schemaMgr = SchemaManager.GetManager();
				schemaMgr.Initialize();

				ConfigManager configMgr = ConfigManager.GetManager();
				configMgr.Initialize();

				DataObjectCacheManager dataObjectCacheManager = DataObjectCacheManager.GetManager();
				dataObjectCacheManager.Initialize();

				SessionManager sessionMgr = SessionManager.GetManager();
				sessionMgr.Initialize();

				StringManager stringMgr = StringManager.GetManager();
				stringMgr.Initialize();

				TemplateManager templateMgr = TemplateManager.GetManager();
				templateMgr.Initialize();
	
				// Registration and initialization of objects below might require remote calls (need for transactions):
				DataObject.Register();
				UserDataObject.Register();
				CalendarDataObject.Register();
				TaskDataObject.Register();
				ProcessMapDataObject.Register();

				LevelSecurityGuard.Register();
				ObjectActionSecurityGuard.Register();	

				// First initialize base process (standard web functions such as login and logoff, etc
				Process.Initialize();

				// Load all dynamic web plugins (each one contains modules which contain functions)
				LoadPlugins();
				LogManager.Log(this, Engine.Def.ENGINE_NAME + " web service started");
			} 
			catch (Exception ex)
			{
				if (logMgr != null && logMgr.IsInitialized())
					LogManager.Log(this, ex.ToString(), LogManager.Level.ERROR);

				throw ex;
			}
		}


		/// <summary>
		/// Loads plugins.
		/// Attempts to read in every .dll file in the plugin path.
		/// Within each plugin "WebPlugin" type is searched for. 
		/// When found, "Load()" method is called.
		/// </summary>
		protected void LoadPlugins()
		{
			// Find where the web plugins are being loaded from:
			SettingManager settingMgr = SettingManager.GetManager();
			string pluginPath = settingMgr[SETTING_WEBPLUGINSPATH];
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

				// Enusre this is a valid web plugin
				Type[] assemblyTypes = assembly.GetTypes();
				IEnumerator types = assemblyTypes.GetEnumerator();
				while (types.MoveNext())
				{
					Type pluginType = (Type) types.Current;
					if (pluginType.Namespace + ".WebPlugin" == pluginType.FullName)
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

		/// <summary>
		/// Session start handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Session_Start(Object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Begin request handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}


		/// <summary>
		/// End request handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Authenticate request handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{


		}


		/// <summary>
		/// Application error handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Application_Error(Object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Session end error handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Session_End(Object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Application end error handler
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void Application_End(Object sender, EventArgs e)
		{
			LogManager logMgr = LogManager.GetManager();
			try
			{
				// Un-Initialize managers:
				TemplateManager templateMgr = TemplateManager.GetManager();
				templateMgr.UnInitialize();

				StringManager stringMgr = StringManager.GetManager();
				stringMgr.UnInitialize();

				SessionManager sessionMgr = SessionManager.GetManager();
				sessionMgr.UnInitialize();

				ConfigManager configMgr = ConfigManager.GetManager();
				configMgr.UnInitialize();

				SchemaManager schemaMgr = SchemaManager.GetManager();
				schemaMgr.UnInitialize();

				SecurityManager securityMgr = SecurityManager.GetManager();
				securityMgr.UnInitialize();

				EnvironmentManager environmentMgr = EnvironmentManager.GetManager();
				environmentMgr.UnInitialize();

				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				serverConnectionMgr.UnInitialize();
				
				LockManager	lockMgr = LockManager.GetManager();
				lockMgr.UnInitialize();

				logMgr.UnInitialize();		
			
				SettingManager settingMgr = SettingManager.GetManager();
				settingMgr.UnInitialize();
			}
			catch(Exception error)
			{
				if (logMgr != null && logMgr.IsInitialized())
					LogManager.Log(this, "Server Startup Error: " + error.ToString());

				throw error;
			} 		
		}	
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

