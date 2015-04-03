using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Threading;

namespace Engine
{
	/// <summary>
	/// Schema manager. Keeps track of column types of requested
	/// </summary>
	public class SchemaManager : Manager
	{
		
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected SchemaManager g_manager;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 	

		/// <summary>
		/// Synch lock on the m_environmentSchemas operations
		/// </summary>
		protected Hashtable m_envLocks;
	
		/// <summary>
		/// Environment schemas sets hash
		/// </summary>
		protected Hashtable m_environmentSchemas;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static SchemaManager()
		{
			g_manager = new SchemaManager();
		}


		/// <summary>
		/// Default constructor. Creates empty environment schema sets list
		/// </summary>
		protected SchemaManager()
		{			
			m_environmentSchemas = new Hashtable();
			m_envLocks = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static SchemaManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes instance of schema manager:
		/// </summary>
		public override void Initialize()
		{	
			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			m_location = envMgr.GetLocation();

			// Create a lock for each environment
			IDictionaryEnumerator itr = envMgr.GetEnvironments();
			while (itr.MoveNext())
			{
				Environment environment = (Environment) itr.Value;
				m_envLocks[environment.ID] = new ReaderWriterLock();
			}

			// All schemas are pulled on-demand so there is nothing we need to do here.
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns schema definition for schema object within given environment.
		/// Performs get by reference (unsafe)
		/// </summary>		
		/// <param name="schemaName">Schema name (e.g. table name)</param>
		/// <param name="transaction">Transaction</param>
		/// <returns>Schema definition</returns>
		/// <exception cref="ENotInitialized">When manager is not initialized</exception>
		/// <exception cref="EInvalidParameter">When either one of the arguments is empty or invalid</exception>
		public Schema GetSchema(string schemaName, string environmentID)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (schemaName == null)
				throw new EInvalidParameter(this, "schemaName");

			ReaderWriterLock readerWriterLock = (ReaderWriterLock) m_envLocks[environmentID];
			if (readerWriterLock == null)
				throw new EInvalidParameter(this, "environmentID");
			
			Schema schema;

			readerWriterLock.AcquireReaderLock(int.MaxValue);

			string formattedSchemaName = schemaName.ToLower();

			// Find schema definition set:
			Hashtable schemas = (Hashtable) m_environmentSchemas[environmentID];
			if (schemas == null)
			{
				try
				{
					readerWriterLock.ReleaseReaderLock();
					readerWriterLock.AcquireWriterLock(int.MaxValue);

					// First call within this environment, create set:
					m_environmentSchemas[environmentID] = new Hashtable();
					schemas = (Hashtable) m_environmentSchemas[environmentID];
					schema = PullSchema(formattedSchemaName, environmentID);
					schemas[formattedSchemaName] = schema;
				
					readerWriterLock.ReleaseWriterLock();
				}
				catch (Exception e)
				{
					readerWriterLock.ReleaseWriterLock();
					throw e;				
				}				
			}
			else
			{
				schema = (Schema) schemas[formattedSchemaName];
				if (schema == null)
				{
					try
					{
						readerWriterLock.ReleaseReaderLock();
						readerWriterLock.AcquireWriterLock(int.MaxValue);

						// Schema not cached yet so we pull it:
						schema = PullSchema(formattedSchemaName, environmentID);
						schemas[formattedSchemaName] = schema;

						readerWriterLock.ReleaseWriterLock();
					}
					catch (Exception e)
					{
						readerWriterLock.ReleaseWriterLock();
						throw e;				
					}	
				}
				else
					readerWriterLock.ReleaseReaderLock();

			}		
			return schema;
		}

		public Schema GetSchemaRemote(Transaction transaction, string schemaName, string environmentID)
		{
			return GetSchema(schemaName, environmentID);
		}


		/// <summary>
		/// Causes the manager to refresh list of its schema definitions for all environments
		/// </summary>
		public void Refresh()
		{
			m_environmentSchemas = new Hashtable();
		}


		/// <summary>
		/// Causes the manager to refresh list of its schema definitions for given environment
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <exception cref="EInvalidParameter">If environment ID is null</exception>
 		public void Refresh(string environmentID)
		{
			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			m_environmentSchemas[environmentID] = null;
		}


		/// <summary>
		/// Retreives schema definition for given schema name within given schema environment.
		/// Retreived schema instance is not stored anywhere.
		/// </summary>		
		/// <param name="schemaName">Schema name (e.g. table name)</param>
		/// <param name="transaction">Transaction</param>
		/// <returns>Scheme definition</returns>
		protected Schema PullSchema(string schemaName, string environmentID)
		{
			Schema s;
			if (m_location == Location.APP_SERVER)
			{
				try
				{
					using (Transaction transaction = new Transaction(environmentID))
					using (IDataReader reader = transaction.GetDBConnection().ExecuteRead("select * from " + schemaName + " where 0 = 1"))
					{
						s = new Schema(schemaName);

						// Using data reader find definition of the requested schema element:

						// Find .ADO data type for every returned column:
						for (int count = 0; count < reader.FieldCount; count++) 
						{
							string columnName = reader.GetName(count);
							Type columnType = reader.GetFieldType(count);

							DataValue.DataType dataType = DataValue.DataType.Uknown;
							if (columnType == Type.GetType("System.String"))
								dataType = DataValue.DataType.String;
							else if (columnType == Type.GetType("System.DateTime"))
								dataType = DataValue.DataType.DateTime;
							else if (columnType == Type.GetType("System.Int32"))
								dataType = DataValue.DataType.Long;
							else if (columnType == Type.GetType("System.Double"))
								dataType = DataValue.DataType.Double;
							else if (columnType == Type.GetType("System.Int16"))
								dataType = DataValue.DataType.Flag;

							s[columnName.ToLower()] = dataType;
						}
					}
				}
				catch(Exception exception)
				{
					throw new EInvalidParameter(this, "schemaName", schemaName, "Error occured while pulling schema in environment " + environmentID + ": " + exception.ToString());
				}
			}
			else		
			{
				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				ServerConnectionProxy proxy = serverConnectionMgr.GetServerConnection();
				Object[] args = new Object[3];
				args[1] = schemaName;
				args[2] = environmentID;
				s = (Schema) proxy.Send(null, GetType().ToString(), "GetSchemaRemote", args);
			}

			return s;
		} 
	}
}
