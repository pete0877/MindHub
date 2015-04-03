using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Engine
{
	[Serializable]
	public class UserDataObject : DataObject
	{		
		/// <summary>
		/// Default config ID for any instances of this class
		/// </summary>
		static public string CONFIGID_DEFAULT			= "1";

		/// <summary>
		/// Schema in default configuration.
		/// </summary>
		static public string SCHEMA_DEFAULT				= "TUser";

		/// <summary>
		/// Username column
		/// </summary>
		static public string COLUMN_USERNAME = "usernametxt";

		/// <summary>
		/// Password column
		/// </summary>
		static public string COLUMN_PASSWORD = "passwordtxt";

		/// <summary>
		/// Language column
		/// </summary>
		static public string COLUMN_LANGCD = "langcd";

		/// <summary>
		/// Style column
		/// </summary>
		static public string COLUMN_STYLECD = "stylecd";

		/// <summary>
		/// Singelton delegator instance reference
		/// </summary>
		new public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Class ID
		/// </summary>
		new protected static string g_classID = "Engine.UserDataObject";	

		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		new public static ICreatable CreateInstance()
		{
			return new UserDataObject();
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		new public static void Register()
		{
			g_creator = new CreateObjectDelegator(UserDataObject.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);
		}


		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <return>Class ID</return>
		override public string GetClassID()
		{
			return g_classID;
		}


		/// <summary>
		/// Initializes instance (assumes standard configuration ID)
		/// Assumes new object is being created. CONFIGID_DEFAULT will be used for configuration ID.
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void Initilize(Transaction transaction)
		{
			base.Initilize(CONFIGID_DEFAULT, transaction);
		}


		/// <summary>
		/// Initializes instance with given configuration ID.
		/// Assumes new object is being created.
		/// </summary>
		/// <param name="configID">Configuration ID. If null CONFIGID_DEFAULT will be used.</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		override public void Initilize(string configID, Transaction transaction)
		{
			if (configID == null)
				base.Initilize(CONFIGID_DEFAULT, transaction);
			else
				base.Initilize(configID, transaction);
		}


		/// <summary>
		/// Initializes instance with given object ID and configuration ID.
		/// Assumes existing object will be loaded or deleted.
		/// </summary>
		/// <param name="objectID">Object ID</param>
		/// <param name="configID">Configuration ID. If null CONFIGID_DEFAULT will be used.</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		override public void Initilize(string objectID, string configID, Transaction transaction)
		{
			if (configID == null)
				base.Initilize(objectID, CONFIGID_DEFAULT, transaction);
			else
				base.Initilize(objectID, configID, transaction);
		}


		/// <summary>
		/// Loads the data object using given username and password.
		/// Performs set of the username and password by value (safe).
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null or if the object was initialized with empty object ID</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		public void LoadFromLogin(string username, string password, Transaction transaction)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			if (username == null)
				throw new EInvalidParameter(this, "username");

			if (password == null)
				throw new EInvalidParameter(this, "password");

			// Find the user:
			string table = (string) m_tables[0];

			DataReader reader = new DataReader(table);
			reader += DataOperator.SQL_WILDCARD;
			reader.Where(UserDataObject.COLUMN_USERNAME + " = '" + username + "'");
			reader.Where(UserDataObject.COLUMN_PASSWORD + " = '" + password + "'");
			reader.Load(transaction); 
			if (!reader.Read())
				throw new EInvalidLogin(); // todo: whoever catches it should redirect to error page for the environment
				
			m_elements = reader.GetDataElements();
			m_ID = this[COLUMN_OBJID];
			IsNew = false;

			// Invoke local event handler for derived classes
			OnLoad(transaction);

			// todo: figure out how to make this work:  if (OnLoadEventSender != null)
			// if (OnLoadEventSender != null)
			//	OnLoadEventSender(this, new EventArgs());
		}
	}
}
