using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// LevelSecurityGuard authorzes user to enter levels of security based on 
	/// SecurityManager's map of users to their allowed levels.
	/// </summary>
	public class LevelSecurityGuard : MarshalByRefObject, ILevelSecurityGuard
	{
		// Constants
		static public string SCHEMA_USERLEVELS					= "tsecurityuserlevels";		

		// Constants
		static public string CODESTRINGSET_SECURITYLEVELS		= "4";		

		/// <summary>
		/// Class ID
		/// </summary>
		protected static string g_classID = "Engine.LevelSecurityGuard";

		/// <summary>
		/// Singelton delegator instance reference
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 		

		/// <summary>
		/// Flag for if the guard has been initialized
		/// </summary>
		protected bool m_initialized;	

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LevelSecurityGuard()
		{
		}


		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <return>Class ID</return>
		public string GetClassID()
		{
			return g_classID;
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(LevelSecurityGuard.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);

			SecurityManager.GetManager().RegisterSecurityGuard(g_classID);
		}

		
		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new LevelSecurityGuard();
		}


		/// <summary>
		/// Initialization function.
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		virtual public void Initialize(Transaction transaction)
		{		
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			m_location = envMgr.GetLocation();
			m_initialized = true;
		}


		/// <summary>
		/// Authorized users's access to given level of security
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="level">Level being autorized</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null or if the action is not recognized or expected for this call</exception>
		/// <exception cref="ENotInitialized">If the guard has not been initialized.</exception>
		/// <exception cref="EUnauthorized">If the guard rejects authorization</exception>
		virtual public void Authorize(Engine.Session session, string level)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (session == null)
				throw new EInvalidParameter(this, "session");

			if (level == null)
				throw new EInvalidParameter(this, "level");

			string levelStatus = (DataValue) session[level];
			if (levelStatus == null || levelStatus.Length == 0 || levelStatus != DataValue.TRUE_STRING)
				throw new EUnauthorized(this, "User not on security level: " + level);
		}


		/// <summary>
		/// Returns list of environment IDs for which the security guard is applicable.
		/// </summary>
		/// <return>List of environment IDs or null if the security guard applies to all environments</return>
		virtual public IDictionaryEnumerator GetEnvironments()
		{
			return null;
		}


		/// <summary>
		/// Caches security into user session
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the guard has not been initialized.</exception>
		virtual public void CacheSecurity(Engine.Session session, Transaction transaction)
		{
			Schema schema = SchemaManager.GetManager().GetSchema(SCHEMA_USERLEVELS, transaction.EnvironmentID);
			IDictionaryEnumerator itrColumns = schema.GetEnumerator();

			// Read in the ACL table:
			DataReader reader = new DataReader(SCHEMA_USERLEVELS);
			reader += DataOperator.SQL_WILDCARD;
			string userID = session.UserID;
			reader.Where(UserDataObject.COLUMN_OBJID + " = '" + userID + "'");
			reader.Load(transaction);
			if (reader.Read())
			{					
				while (itrColumns.MoveNext())
				{
					string column = (string) itrColumns.Key;
					session[column] = reader[column];
				}	
			}	
		}
	}
}

