using System;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Interface for object that can store itself in a database using datbase connection.
	/// </summary>
	public interface IDBStorable
	{
		/// <summary>
		/// Saves self to DB
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void SaveToDB(DBConnection connection);


		/// <summary>
		/// Loads self from DB
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void LoadFromDB(DBConnection connection);


		/// <summary>
		/// Deletes self from DB
		/// </summary>
		/// <param name="connection">connection</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void DeleteFromDB(DBConnection connection);
	}
}
