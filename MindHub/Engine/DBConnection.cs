using System;
using System.Data;
using System.Data.SqlClient;

namespace Engine
{
	/// <summary>
	/// Generic database connection wrapper class
	/// </summary>
	public class DBConnection : IDisposable
	{
		/// <summary>
		/// Connection state
		/// </summary>
		public enum State { Used, Free }

		/// <summary>
		/// Database type
		/// </summary>
		public enum DBType { SQLServer, Oracle, DB2, MSAccess }

		/// <summary>
		/// SQL Server connection instance
		/// </summary>
		protected SqlConnection m_sqlConnection;

		/// <summary>
		/// DB type this connection goes to
		/// </summary>
		protected DBType m_dbType;

		/// <summary>
		/// Environment ID of this connection (cached)
		/// </summary>
		public string EnvironmentID;

		/// <summary>
		/// ID of this connection
		/// </summary>
		public string ID;

		/// <summary>
		/// Usage state (if it is currently used by any thread)
		/// </summary>
		protected State m_state;

		/// <summary>
		/// Initiates the connection object using connection string
		/// </summary>
		/// <param name="connectionID">Connection ID (given by the connection manager)</param>
		/// <param name="environment">Envrironment to connect to</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public DBConnection(string connectionID, Environment environment)
		{
			if (environment == null)
				throw new EInvalidParameter(this, "environment");

			if (connectionID == null)
				throw new EInvalidParameter(this, "connectionID");


			ID = string.Copy(connectionID);		
			EnvironmentID = string.Copy(environment.ID);

			m_dbType = environment.DBType;
			switch (m_dbType)
			{
				case DBType.SQLServer:
					m_sqlConnection = new SqlConnection(environment.DNS);
					m_sqlConnection.Open();				
					break;
				default:
					throw new Exception("DB type not supported");
			}			
		}


		/// <summary>
		/// Standard destructor
		/// </summary>
		~DBConnection()
		{
			if (m_sqlConnection != null)
				m_sqlConnection.Dispose();			
		}


		/// <summary>
		/// Disposes self by freeing up its status (using db connection manager)
		/// </summary>
		public void Dispose()
		{
			DBConnectionManager dbConnectionMgr = DBConnectionManager.GetManager();
			dbConnectionMgr.ReleaseConnection(this);
		}


		/// <summary>
		/// Sets current connection usage state
		/// </summary>
		/// <param name="state">State</param>
		public void SetState(State state)
		{
			m_state = state;
		}


		/// <summary>
		/// Returns current conneciton usage state
		/// </summary>
		/// <returns></returns>
		public State GetState()
		{
			return m_state;
		}


		/// <summary>
		/// Executes SQL statement and returns a reader object
		/// </summary>
		/// <param name="sql">SQL Statement</param>
		/// <returns>Reader object</returns>
		/// <exception cref="EInvalidParameter">If sql string is empty or null</exception>
		/// <exception cref="ENotInitialized">If the internal connection is not initialized correctly</exception>
		public IDataReader ExecuteRead(string sql)
		{
			if (sql == null || sql.Length == 0)
				throw new EInvalidParameter(this, "sql", sql, "");

			// Depending on the type of database, we will get our data reader from different internal connections:
			try
			{
				switch (m_dbType)
				{
					case DBType.SQLServer:
						if (m_sqlConnection == null)
							throw new ENotInitialized(this);
						
						SqlCommand command = new SqlCommand(sql, m_sqlConnection);
						IDataReader reader = command.ExecuteReader();
						return reader;
					default:
						throw new Exception("DB type not supported");
				}
			}
			catch (Exception exception)
			{
				throw new Exception("While executing reader SQL (" + sql + ")", exception);
			}
		}


		/// <summary>
		/// Executes SQL statement
		/// </summary>
		/// <param name="sql"></param>
		/// <exception cref="EInvalidParameter">If sql string is empty or null</exception>
		/// <exception cref="ENotInitialized">If the internal connection is not initialized correctly</exception>
		public void ExecuteSQL(string sql)
		{
			if (sql == null || sql.Length == 0)
				throw new EInvalidParameter(this, "sql", sql, "");

			// Depending on the type of database, we will get our command object differently:
			try
			{
				switch (m_dbType)
				{
					case DBType.SQLServer:
						if (m_sqlConnection == null)
							throw new ENotInitialized(this);
						
						SqlCommand command = new SqlCommand(sql, m_sqlConnection);
						command.ExecuteNonQuery();
						break;						
					default:
						throw new Exception("DB type not supported");
				}
			}
			catch (Exception exception)
			{
				throw new Exception("While executing SQL (" + sql + ")", exception);
			}
		}
	}
}
