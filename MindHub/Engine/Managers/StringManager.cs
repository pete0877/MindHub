using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;


namespace Engine
{
	public class StringManager : Manager
	{
		/// <summary>
		/// Default string value returned if referenced string does not exist
		/// </summary>
		static public string DEFAULT_STRING			= "";

		/// <summary>
		/// String set ID used for global set of strings
		/// </summary>
		static public string DESCRIPTIONS_STRING_SET_ID		= "0";

		/// <summary>
		/// String set ID used for global set of strings
		/// </summary>
		static public string GLOBAL_STRING_SET_ID			= "1";

		/// <summary>
		/// Name of the table storing the configurations
		/// </summary>
		static public string SCHEMA_STRINGS			= "tstring";

		/// <summary>
		/// String set ID column
		/// </summary>
		static public string COLUMN_SETID			= "setid";

		/// <summary>
		/// String ID column
		/// </summary>
		static public string COLUMN_STRINGID		= "stringid";

		/// <summary>
		/// Language code column
		/// </summary>
		static public string COLUMN_LANGCD			= "langcd";

		/// <summary>
		/// Language code column
		/// </summary>
		static public string COLUMN_STRINGTXT		= "stringtxt";

        
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected StringManager g_manager;

		/// <summary>
		/// Proxy (reference to application server's instance of this class)
		/// </summary>
		static protected StringManager g_proxy;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 		

		/// <summary>
		/// Hash of string sets keyed by environment IDs
		/// </summary>
		protected Hashtable m_enviroments;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static StringManager()
		{
			g_manager = new StringManager();
		}


		/// <summary>
		/// Default constructor. Creates empty configurations list.
		/// </summary>
		protected StringManager()
		{			
			m_enviroments = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static StringManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes strings hashes for all environments
		/// </summary>
		public override void Initialize()
		{
			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			m_location = envMgr.GetLocation();

			// If the local environment is on app server, load the strings for each environment
			// from database. Else get all strings from remote call:
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
						// Create new LANGCD-to-SETID hash for each environment:
						Hashtable languages = new Hashtable();
						m_enviroments[environmentID] = languages;					

						// As we read the contents of the string table, we will compose the lower structures:
						DataReader reader = new DataReader(SCHEMA_STRINGS);
						reader += DataReader.SQL_WILDCARD;
						for (reader.Load(transaction); reader.Read();)
						{							
							string setID = reader[COLUMN_SETID];
							string stringID = reader[COLUMN_STRINGID];
							string langCd = reader[COLUMN_LANGCD];
							string stringTxt = reader[COLUMN_STRINGTXT];

							// Find or create SETID to Strings hash for this language:
							Hashtable sets = (Hashtable) languages[langCd];
							if (sets == null)
							{
								sets = new Hashtable();
								languages[langCd] = sets;
							}
								
							// Find or create string set for the given set ID:
							NameValueCollection strings = (NameValueCollection) sets[setID];
							if (strings == null)
							{
								strings = new NameValueCollection();
								sets[setID] = strings;
							}

							strings[stringID] = stringTxt;
						}
					}
				}
			}
			else		
			{
				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				ServerConnectionProxy proxy = serverConnectionMgr.GetServerConnection();
				m_enviroments = (Hashtable) proxy.Send(null, GetType().ToString(), "GetStateRemote", new Object[1]);				
			}

			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns state of the manager.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns>Current state</returns>
		public Hashtable GetStateRemote(Transaction transaction)
		{
			return m_enviroments;
		}


		/// <summary>
		/// Returns string referenced by given parameters.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="language">Language code</param>
		/// <param name="setID">String set ID</param>
		/// <param name="stringID">String ID</param>
		/// <returns>Found string or if any of the parameters don't match what's cached, empty string</returns>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public string GetString(string environmentID, string language, string setID, string stringID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (language == null)
				throw new EInvalidParameter(this, "language");
			
			if (setID == null)
				throw new EInvalidParameter(this, "setID");

			if (stringID == null)
				throw new EInvalidParameter(this, "stringID");

			Hashtable languages = (Hashtable) m_enviroments[environmentID];
			if (languages == null)
				return DEFAULT_STRING;

			Hashtable sets = (Hashtable) languages[language];
			if (sets == null)
				return DEFAULT_STRING;

			NameValueCollection strings = (NameValueCollection) sets[setID];
			if (strings == null)
				return DEFAULT_STRING;

			string stringsTxt = strings[stringID];
			if (stringsTxt == null)
				return DEFAULT_STRING;

			return stringsTxt;
		}


		/// <summary>
		/// Returns string name+value collection referenced by given parameters.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="language">Language code</param>
		/// <param name="setID">String set ID</param>
		/// <returns>Found collection or if any of the parameters don't match what's cached, empty collection</returns>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public NameValueCollection GetStringSet(string environmentID, string language, string setID)
		{

			/*
				if (m_location == Location.WEB_SERVER)
				{
					g_proxy.Initialize();
					NameValueCollection test = g_proxy.GetStringSet(environmentID, language, setID);			
				}
			*/


			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (language == null)
				throw new EInvalidParameter(this, "language");
			
			if (setID == null)
				throw new EInvalidParameter(this, "setID");

			Hashtable languages = (Hashtable) m_enviroments[environmentID];
			if (languages == null)
				return new NameValueCollection();

			Hashtable sets = (Hashtable) languages[language];
			if (sets == null)
				return new NameValueCollection();

			NameValueCollection strings = (NameValueCollection) sets[setID];
			if (strings == null)
				return new NameValueCollection();

			return strings;
		}


		/// <summary>
		/// Returns string set description.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="language">Language code</param>
		/// <param name="setID">String set ID</param>
		/// <returns>Found collection or if any of the parameters don't match what's cached, empty collection</returns>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public string GetStringSetDescription(string environmentID, string language, string setID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (language == null)
				throw new EInvalidParameter(this, "language");
			
			if (setID == null)
				throw new EInvalidParameter(this, "setID");

			Hashtable languages = (Hashtable) m_enviroments[environmentID];
			if (languages == null)
				return DEFAULT_STRING;

			Hashtable sets = (Hashtable) languages[language];
			if (sets == null)
				return DEFAULT_STRING;

			NameValueCollection stringSetDescriptions = (NameValueCollection) sets[StringManager.DESCRIPTIONS_STRING_SET_ID];
			if (stringSetDescriptions == null)
				return DEFAULT_STRING;

			string setDescription = stringSetDescriptions[setID];
			if (setDescription == null)
				return DEFAULT_STRING;

			return setDescription;
		}
	}
}
