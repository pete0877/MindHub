using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// User session. Stores basic information needed to recognize the user and the environment user is in.
	/// </summary>
	public class Session
	{
		/// <summary>
		/// Session ID
		/// </summary>
		public string ID;

		/// <summary>
		/// Cached environment ID
		/// </summary>
		public string EnvironmentID;

		/// <summary>
		/// Cached user ID
		/// </summary>
		public string UserID;

		/// <summary>
		/// Cached user's language code
		/// </summary>
		public string LanguageCd;

		/// <summary>
		/// Cached user's ui style code
		/// </summary>
		public string StyleCd;

		/// <summary>
		/// Hash of session variables (any system.object can be stored here)
		/// </summary>
		protected Hashtable m_hash;

		/// <summary>
		/// List of groups user belongs to
		/// </summary>
		protected Hashtable m_groups;

		/// <summary>
		/// OS path to the UI directory, including language and style folder names (e.g. 'D:\MindHub\Dev\WebRoot\Fake\eng\orange'). No slash at the end.
		/// </summary>
		protected string m_uiPathOS;

		/// <summary>
		/// Web path to the UI directory, including language and style folder names (e.g. '/fake/eng/orange/html'). No slash at the end.
		/// </summary>
		protected string m_uiPathWeb;

		/// <summary>
		/// Constructor
		/// Performs set by value of the intput parameters (safe).
		/// </summary>
		public Session(string environmentID, string sessionID)
		{			
			m_hash = new Hashtable();
			m_groups = new Hashtable();
			ID = string.Copy(sessionID);
			EnvironmentID = string.Copy(environmentID);
		}


		/// <summary>
		/// Session variable access operator. 
		/// Performs set/get by value (safe).
		/// </summary>
		/// <param name="variable">Name of the session variable</param>		
		/// <returns>Session variable value or null string if session variable does not exist</returns>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		public object this[string variable]
		{
			get
			{				
				if (variable == null)
					throw new EInvalidParameter(this, "variable");

				if (!m_hash.ContainsKey(variable))
					return null;

				return m_hash[variable];
			}
			set
			{				
				if (variable == null)
					throw new EInvalidParameter(this, "variable");

				m_hash[variable] = value;
			}			
		}


		/// <summary>
		/// Checks if given variable exists in the session.
		/// </summary>
		/// <param name="variable">Variable name</param>
		/// <returns>Flag for if variable is set in the session</returns>
		public bool ContainsObject(string variable)
		{
			if (variable == null)
				throw new EInvalidParameter(this, "variable");

			return m_hash.ContainsKey(variable);
		}


		/// <summary>
		/// Returns flag for if user is in a given group
		/// </summary>
		/// <param name="group">Group name</param>
		/// <returns>Flag if user is in the given group</returns>
		/// <exception cref="EInvalidParameter">If any of the parameter is null or empty</exception>
		public bool IsInGroup(string group)
		{
			if (group == null || group == "")
				throw new EInvalidParameter(this, "group");

			return m_groups.Contains(group);
		}


		/// <summary>
		/// Adds user to the given group
		/// </summary>
		/// <param name="group">Group name</param>
		/// <exception cref="EInvalidParameter">If any of the parameter is null or empty</exception>
		public void AddToGroup(string group)
		{
			if (group == null || group == "")
				throw new EInvalidParameter(this, "group");

			m_groups[group] = true;
		}


		/// <summary>
		/// Attaches self to given user data object. This function is called upon login right after user object has been retreived based on username and password.
		/// </summary>
		/// <param name="userDataObject"></param>
		public void AttachTo(UserDataObject userDataObject)
		{
			this[UserDataObject.COLUMN_USERNAME] = userDataObject[UserDataObject.COLUMN_USERNAME];
			this[UserDataObject.COLUMN_PASSWORD] = userDataObject[UserDataObject.COLUMN_PASSWORD];
			UserID = userDataObject[UserDataObject.COLUMN_OBJID];
			LanguageCd = userDataObject[UserDataObject.COLUMN_LANGCD];
			StyleCd = userDataObject[UserDataObject.COLUMN_STYLECD];

			// Find UI paths (OS and web):
			Environment environment = EnvironmentManager.GetManager().GetEnvironment(EnvironmentID);						
			m_uiPathOS = environment.UIPathOS + LanguageCd + "\\" + StyleCd;
			m_uiPathWeb = environment.UIPathWeb + LanguageCd + "/" + StyleCd;
		}

		/// <summary>
		/// Returns the OS path to the UI folder (see m_uiPathOS).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>OS path to the UI folder</returns>
		public string GetUIPathOS()
		{
			return m_uiPathOS;
		}

		/// <summary>
		/// Returns the Web path to the UI folder (see m_uiPathWeb).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Web path to the UI folder</returns>
		public string GetUIPathWeb()
		{
			return m_uiPathWeb;
		}
	}
}
