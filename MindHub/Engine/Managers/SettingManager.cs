using System;
using System.Collections;
using System.Configuration;

namespace Engine
{
	/// <summary>
	/// This class is responsible for abstracting access to .NET config files. 
	/// Although this isn't done currently, in future remote calls between web and 
	/// app server can be implemented to be able to access settings that should be shared
	/// between the two executables.
	/// </summary>
	public class SettingManager : Manager
	{
		/// <summary>
		/// Configuration file section for web server
		/// </summary>
		static public string SETTING_SECTION_WEBSERVER		= "webServer";

		/// <summary>
		/// Configuration file section for application server
		/// </summary>
		static public string SETTING_SECTION_APPSERVER		= "applicationServer";

		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected SettingManager g_manager;

		/// <summary>
		/// Dictionary of settings for the assembly
		/// </summary>
		protected IDictionary m_settings;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 		

	
		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static SettingManager()
		{
			g_manager = new SettingManager();
		}		


		/// <summary>
		/// Default constructor
		/// </summary>
		public SettingManager()
		{
			m_settings = null;
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
		/// Sets the locaiton with which this instance is initialized.
		/// </summary>
		/// <param name="location">Location (e.g. Location.APP_SERVER)</param>
		public void SetLocation(Location location)
		{			
			m_location = location;
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static SettingManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes instance of this manager by connecting to configuration file (.config for the assembly)
		/// </summary>
		/// <param name="location">Location of where the initialization is taking place</param>
		public override void Initialize()
		{			
			if (m_location == Engine.Location.APP_SERVER)
			{
				m_settings = (IDictionary) ConfigurationSettings.GetConfig(SETTING_SECTION_APPSERVER);
				if (m_settings == null)
					throw new Exception("Application server cofiguration file missing or misformatted (e.g. missing section '" + SETTING_SECTION_APPSERVER + "')");
			}
			else
			{
				m_settings = (IDictionary) ConfigurationSettings.GetConfig(SETTING_SECTION_WEBSERVER);
				if (m_settings == null)
					throw new Exception("Web server cofiguration file missing or misformatted (e.g. missing section '" + SETTING_SECTION_WEBSERVER + "')");			
			}

			base.Initialize();
		}


		/// <summary>
		/// Basic access indexer for settings.
		/// Performs set/get by value (safe).
		/// </summary>
		/// <param name="setting">Setting name</param>		
		/// <exception cref="ENotInitialized">If instance of this manger was not initialized yet.</exception>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public string this[string setting]
		{
			get
			{				
				if (setting == null)
					throw new EInvalidParameter(this, "setting");

				if (!m_initialized)
					throw new ENotInitialized(this);

				if (m_settings.Contains(setting))
					return (string) m_settings[setting];
				else
					return "";
			}
			set
			{				
				throw new Exception("SettingManager does not perform settings update on run-time.");
			}			
		}

		/// <summary>
		/// Returns dictionary of settings that fall under given section name of the settings file.
		/// Performs set/get by value (safe).
		/// </summary>
		/// <param name="sectionName">Section name</param>		
		/// <exception cref="ENotInitialized">If instance of this manger was not initialized yet.</exception>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public IDictionary GetSection(string sectionName)
		{
			if (sectionName == null)
				throw new EInvalidParameter(this, "sectionName");

			if (!m_initialized)
				throw new ENotInitialized(this);

			IDictionary section = (IDictionary) ConfigurationSettings.GetConfig(sectionName);
			if (section == null)
				section = new Hashtable();

			return section;	
		}
	}
}
