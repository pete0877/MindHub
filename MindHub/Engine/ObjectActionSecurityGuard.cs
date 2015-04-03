using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// ObjectActionSecurityGuard authorzes users to perform actions on objects based object's
	/// security mappings
	/// </summary>
	public class ObjectActionSecurityGuard : MarshalByRefObject, IObjectActionSecurityGuard
	{
		/// <summary>
		/// ACL user spec prefix
		/// </summary>
		static public string PREFIX_USER = "usr";

		/// <summary>
		/// ACL group spec prefix
		/// </summary>
		static public string PREFIX_GROUP = "grp";

		/// <summary>
		/// ACL spec action delimiter
		/// </summary>
		static public char DELIMITER_ACTION = '.';

		/// <summary>
		/// ACL spec item delimiter
		/// </summary>
		static public char DELIMITER_SPEC = '_';

		/// <summary>
		/// Class ID
		/// </summary>
		protected static string g_classID = "Engine.ObjectActionSecurityGuard";

		/// <summary>
		/// Singelton delegator instance reference
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Flag for if the guard has been initialized
		/// </summary>
		protected bool m_initialized;	

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ObjectActionSecurityGuard()
		{
		}


		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns>Class ID</returns>
		public string GetClassID()
		{
			return g_classID;
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(ObjectActionSecurityGuard.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);

			SecurityManager.GetManager().RegisterSecurityGuard(g_classID);
		}

		
		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new ObjectActionSecurityGuard();
		}


		/// <summary>
		/// Initialization function.
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		virtual public void Initialize(Transaction transaction)
		{		
			m_initialized = true;
		}


		/// <summary>
		/// Authozes users's action on object
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="action">Action</param>
		/// <param name="dataObject">Data object</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the guard has not been initialized.</exception>
		/// <exception cref="EUnauthorized">If the guard rejects authorization</exception>		
		virtual public void Authorize(Engine.Session session, string action, IDataObject dataObject)		
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (session == null)
				throw new EInvalidParameter(this, "session");

			if (action == null)
				throw new EInvalidParameter(this, "action");

			if (dataObject == null)
				throw new EInvalidParameter(this, "dataObject");

			string actionSearched = DELIMITER_ACTION + action + DELIMITER_ACTION;

			// Read security settings from the data object:
			StringDataSet securitySettings = dataObject.GetOperatorSettings(g_classID);
			IEnumerator itrRights = securitySettings.GetValues();
			while (itrRights.MoveNext())
			{
				string key = (string) itrRights.Current;
				string val = securitySettings[key];

				string[] specification = key.Split(DELIMITER_SPEC);
				if (specification.Length != 2)
 					throw new Exception("Data object's security settings formatted incorrectly for guard: " + g_classID);

				string prefix = specification[0];
				string entity = specification[1];
				
				if (prefix == PREFIX_USER)
				{
					// If action is "X" and we find usr_234=.R.W.X. then user is authorized to perform that action
					if (entity == session.UserID && val.IndexOf(actionSearched) >= 0)
						return;				
				} 
				else if (prefix == PREFIX_GROUP)
				{
					// If action is "X" and we find grp_admins=.R.W.X. and user is in 'admins' group, then user is authorized to perform that action
					if (session.IsInGroup(entity) && val.IndexOf(actionSearched) >= 0)
						return;
				}
				else 
					throw new Exception("Data object's security settings formatted incorrectly for guard: " + g_classID);
			}

			throw new EUnauthorized(this, "Data object ACL contains no authorization for user to perform action: " + action);
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
			// TODO: Implement ObjectActionSecurityGuard.CacheSecurity() - needs to find all groups that the user belongs to and put the string of them in the session
		}
	}
}
