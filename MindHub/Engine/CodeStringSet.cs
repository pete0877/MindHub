using System;
using System.Collections;
using System.Collections.Specialized;

namespace Engine
{
	/// <summary>
	/// Class responsible for wraping functionality of getting specific code string set.
	/// Simply contructs itself through call to StringManager
	/// </summary>
	public class CodeStringSet
	{
		/// <summary>
		/// Internal cached value of the string set
		/// </summary>
		protected NameValueCollection m_stringSet;

		/// <summary>
		/// Set ID
		/// </summary>
		protected string m_setID;

		/// <summary>
		/// Language code
		/// </summary>
		protected string m_languageCd;

		/// <summary>
		/// Environment ID
		/// </summary>
		protected string m_environmentID;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="stringSetID">Set ID</param>
		public CodeStringSet(Engine.Session session, string stringSetID)
		{
			Initialize(session.EnvironmentID, session.LanguageCd, stringSetID);
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		public CodeStringSet(Transaction transaction, string languageCd, string stringSetID)
		{
			Initialize(transaction.EnvironmentID, languageCd, stringSetID);
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		public CodeStringSet(string environmentID, string languageCd, string stringSetID)
		{
			Initialize(environmentID, languageCd, stringSetID);
		}


		/// <summary>
		/// Returns enumeration of strings within the set.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Enumeration of string values</returns>
		public IEnumerator GetEnumerator()
		{
			return m_stringSet.GetEnumerator();
		}


		/// <summary>
		/// Returns description of the string set.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>String description of the string set</returns>
		public string GetDescription()
		{
			return StringManager.GetManager().GetStringSetDescription(m_environmentID, m_languageCd, m_setID);
		}


		/// <summary>
		/// String set indexer. Only Get() is allowed.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public string this[string stringID]
		{
			get
			{
				if (stringID == null)
					throw new EInvalidParameter(this, "stringID");

				return m_stringSet[stringID];
			}
			set 
			{
				throw new Exception("Cannot modify string set within CodeStringSet");
			}			
		}


		/// <summary>
		/// Initializes instance of this class by getting the cached string set from StringManager
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		protected void Initialize(string environmentID, string languageCd, string stringSetID)
		{
			StringManager stringMgr = StringManager.GetManager();
			m_stringSet = stringMgr.GetStringSet(environmentID, languageCd, stringSetID);
			m_setID = stringSetID;
			m_languageCd = languageCd;
			m_environmentID = environmentID;
		}
	}
}
