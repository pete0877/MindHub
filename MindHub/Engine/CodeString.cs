using System;

namespace Engine
{
	/// <summary>
	/// Class responsible for wraping functionality of getting specific code string.
	/// Simply contructs itself through call to StringManager
	/// </summary>
	public class CodeString
	{
		/// <summary>
		/// Internal cached value of the string
		/// </summary>
		protected string m_stringTxt;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="stringSetID">Set ID</param>
		/// <param name="stringID">String ID</param>
		public CodeString(Engine.Session session, string stringSetID, string stringID)
		{
			Initialize(session.EnvironmentID, session.LanguageCd, stringSetID, stringID);
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		/// <param name="stringID">String ID</param>
		public CodeString(Transaction transaction, string languageCd, string stringSetID, string stringID)
		{
			Initialize(transaction.EnvironmentID, languageCd, stringSetID, stringID);
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		/// <param name="stringID">String ID</param>
		public CodeString(string environmentID, string languageCd, string stringSetID, string stringID)
		{
			Initialize(environmentID, languageCd, stringSetID, stringID);
		}


		/// <summary>
		/// Constructor. Assumes string set ID of StringManager.GLOBAL_STRING_SET_ID
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="stringID">String ID</param>
		public CodeString(Engine.Session session, string stringID)
		{
			Initialize(session.EnvironmentID, session.LanguageCd, StringManager.GLOBAL_STRING_SET_ID, stringID);
		}


		/// <summary>
		/// Constructor. Assumes string set ID of StringManager.GLOBAL_STRING_SET_ID
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringID">String ID</param>
		public CodeString(Transaction transaction, string languageCd, string stringID)
		{
			Initialize(transaction.EnvironmentID, languageCd, StringManager.GLOBAL_STRING_SET_ID, stringID);
		}


		/// <summary>
		/// Constructor. Assumes string set ID of StringManager.GLOBAL_STRING_SET_ID
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		/// <param name="stringID">String ID</param>
		public CodeString(string environmentID, string languageCd, string stringID)
		{
			Initialize(environmentID, languageCd, StringManager.GLOBAL_STRING_SET_ID, stringID);
		}


		/// <summary>
		/// Initializes instance of this class by getting the cached string from StringManager
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="languageCd">Language code</param>
		/// <param name="stringSetID">Set ID</param>
		/// <param name="stringID">String ID</param>
		protected void Initialize(string environmentID, string languageCd, string stringSetID, string stringID)
		{
			StringManager stringMgr = StringManager.GetManager();
			m_stringTxt = stringMgr.GetString(environmentID, languageCd, stringSetID, stringID);
		}


		/// <summary>
		/// Converts instance of this class to string.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <param name="codeString">CodeString to be converted</param>
		/// <returns>Value returned by conversion</returns>
		public static implicit operator string(CodeString codeString) 
		{
			return codeString.m_stringTxt;
		}
	}
}
