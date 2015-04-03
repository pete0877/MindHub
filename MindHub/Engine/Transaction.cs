using System;
using System.Collections;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Transaction object. Encapsulates cross-server and database transactions.
	/// </summary>
	public class Transaction : IDisposable
	{
		/// <summary>
		/// List of data object actions.
		/// </summary>
		ArrayList m_dataObjectActions;

		/// <summary>
		/// Cached location. 
		/// </summary>
		protected Location m_location;

		/// <summary>
		/// Cached environment ID property
		/// </summary>
		public string EnvironmentID;

		/// <summary>
		/// DB connection on which this transaction operates on (if on app server side)
		/// </summary>
		protected DBConnection m_dbConnection;

		/// <summary>
		/// Server connection on which this transaction operates on (if on web server side)
		/// </summary>
		protected ServerConnectionProxy m_serverConnectionProxy;

		/// <summary>
		/// Flag indicating if the instance of this class has already been disposed (through Dispose() call)
		/// </summary>
		protected bool m_disposed;

		/// <summary>
		/// Flag for if this transaction has already been initialized. 
		/// Transaction object does not initialize until it is necessary. This way transaction
		/// objects can be created quickly and if connection to DB server or applicaiton server is never needed, 
		/// time is saved that would normally be spent on initializing those connections.
		/// </summary>
		protected bool m_initialized;

		/// <summary>
		/// Returns location code.
		/// </summary>
		/// <returns></returns>
		public Location GetLocation()
		{
			return m_location;
		}


		/// <summary>
		/// Constructs self from environment ID
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <exception cref="EInvalidParameter">If environment ID is null or invalid</exception>		
		public Transaction(string environmentID)
		{
			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			EnvironmentID = environmentID;
			m_dataObjectActions = new ArrayList();
			m_initialized = false;
		}


		/// <summary>
		/// Constructs self from session
		/// </summary>
		/// <param name="session">Session instance</param>
		/// <exception cref="EInvalidParameter">If session is null</exception>	
		public Transaction(Engine.Session session)
		{
			if (session == null)
				throw new EInvalidParameter(this, "session");
			
			EnvironmentID = session.EnvironmentID;
			m_dataObjectActions = new ArrayList();
			m_initialized = false;
		}
		

		/// <summary>
		/// Destructor - disposes allocated resources
		/// </summary>
		~Transaction()
		{
			Dispose(false);
		}


		/// <summary>
		/// Disposes allocated resources. This function should be called as soon as the transaction is no longer needed!
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if(!m_disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if(disposing)
				{
					if (m_serverConnectionProxy != null)
					{
						m_serverConnectionProxy.Dispose();
						m_serverConnectionProxy = null;
					}

					if (m_dbConnection != null)
					{
						m_dbConnection.Dispose();
						m_dbConnection = null;
					}					
				}
			}
			m_disposed = true;         
		}


		/// <summary>
		/// Initializes self. This method is called on-demand. It's not called until either 
		/// DB connection or Proxy connections are needed.
		/// </summary>
		protected void Initialize()
		{
			EnvironmentManager envMgr = EnvironmentManager.GetManager();			
			if (envMgr.GetLocation() == Location.APP_SERVER)
			{
				DBConnection connection = DBConnectionManager.GetManager().GetDBConnection(EnvironmentID);
				Initialize(connection);
			}
			else
			{
				ServerConnectionProxy connectionProxy = ServerConnectionManager.GetManager().GetServerConnection();
				Initialize(connectionProxy);
			}		
	
			m_initialized = true;
		}

		/// <summary>
		/// Initializes self given database connection
		/// </summary>
		/// <param name="connection">Database connection</param>
		protected void Initialize(DBConnection connection)
		{
			m_dbConnection = connection;
			m_location = Location.APP_SERVER;			
		}


		/// <summary>
		/// Initializes self with connection to remote transaction (newly created on application server)
		/// </summary>
		protected void Initialize(ServerConnectionProxy connectionProxy)
		{
			m_serverConnectionProxy = connectionProxy;
			m_location = Location.WEB_SERVER;			
		}


		/// <summary>
		/// Return reference to the db connection object
		/// </summary>
		/// <returns>DB Connection Object</returns>
		/// <exception cref="ENotInitialized">If database connection was not initialized</exception>
		public DBConnection GetDBConnection()
		{
			if (!m_initialized)
				Initialize();

			if (m_dbConnection == null)
				throw new ENotInitialized(this);

			return m_dbConnection;
		}


		/// <summary>
		/// Return reference to the server connection object
		/// </summary>
		/// <returns>Server Connection Object</returns>
		/// <exception cref="ENotInitialized">If server connection was not initialized</exception>
		public ServerConnectionProxy GetServerConnection()
		{
			if (!m_initialized)
				Initialize();

			if (m_serverConnectionProxy == null)
				throw new ENotInitialized(this);

			return m_serverConnectionProxy;
		}


		/// <summary>
		/// Sets the reference to the list of data object actions.
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="actions">List of data object actions</param>
		public void SetDataObjectActions(ArrayList actions)
		{
			m_dataObjectActions  = actions;
		}
	
		
		/// <summary>
		/// Returns list of data object actions.
		/// Perfors get by reference (unsafe)
		/// </summary>
		public ArrayList GetDataObjectActions()
		{
			return m_dataObjectActions;
		}


 		/// <summary>
		/// Assists data object in loading itself
		/// </summary>
		/// <param name="dataObject">Data object</param>
		/// <exception cref="EInvalidParameter">If data object pointer is null</exception>	
		public void LoadDataObject(DataObject dataObject)
		{
			if (!m_initialized)
				Initialize();

			if (dataObject == null)
				throw new EInvalidParameter(this, "dataObject");

			// If transaction is executing on application server side, we simply instruct the data object to load itself from database connection
			if (m_location == Location.APP_SERVER)
				dataObject.LoadFromDB(m_dbConnection);
			else
			{
				// User the remote transaction to load the object:
				Object[] args = new Object[4];	// First element will be transaction.
				args[1] = dataObject.GetClassID();
				args[2] = dataObject.GetID();
				args[3] = dataObject.GetConfigID();				
				byte[] array = (byte[]) m_serverConnectionProxy.Send(EnvironmentID, "Engine.DataManager", "LoadDataObject", args);
				MemoryStream stream = new MemoryStream(array);
				dataObject.LoadFromStream(stream);
			}
		}


		/// <summary>
		/// Assists data object in saving itself
		/// </summary>
		/// <param name="dataObject">Data object</param>
		/// <exception cref="EInvalidParameter">If data object pointer is null</exception>	
		public void SaveDataObject(DataObject dataObject)
		{
			if (!m_initialized)
				Initialize();

			if (dataObject == null)
				throw new EInvalidParameter(this, "dataObject");

			// Add data object to the data object action list. This list will be processed when transaction is committed
			bool storeMinimal = false;
			if (m_location != Location.APP_SERVER)
				storeMinimal = true;
				
			if (dataObject.IsNew)
				m_dataObjectActions.Add(new DataObjectAction(DataObjectAction.ActionType.Insert, dataObject, storeMinimal));
			else
				m_dataObjectActions.Add(new DataObjectAction(DataObjectAction.ActionType.Update, dataObject, storeMinimal));
		}


		/// <summary>
		/// Assists data object in deleting itself
		/// </summary>
		/// <param name="dataObject">Data object</param>
		/// <exception cref="EInvalidParameter">If data object pointer is null</exception>	
		public void DeleteDataObject(DataObject dataObject)
		{
			if (!m_initialized)
				Initialize();

			if (dataObject == null)
				throw new EInvalidParameter(this, "dataObject");

			// Add data object to the data object action list. This list will be processed when transaction is committed
			bool storeMinimal = false;
			if (m_location != Location.APP_SERVER)
				storeMinimal = true;

			m_dataObjectActions.Add(new DataObjectAction(DataObjectAction.ActionType.Delete, dataObject, storeMinimal));
		}


		/// <summary>
		/// Commits transaction
		/// </summary>		
		public void Commit()
		{
			if (!m_initialized)
				Initialize();

			// If transaction executes on the server side, run through data object action list:
			if (m_location == Location.APP_SERVER)
			{
				IEnumerator itr = m_dataObjectActions.GetEnumerator();
				while(itr.MoveNext())
				{
					DataObjectAction action = (DataObjectAction) itr.Current;
					if (
						action.GetAction() == DataObjectAction.ActionType.Insert || 
						action.GetAction() == DataObjectAction.ActionType.Update	
					)
						action.GetDataObject().SaveToDB(m_dbConnection);
					else
						action.GetDataObject().DeleteFromDB(m_dbConnection);
				}
			}
			else
			{
				if (m_dataObjectActions.Count > 0)
				{
					Object[] args = new Object[2];	// First element will be transaction.
					args[1] = m_dataObjectActions;
					m_serverConnectionProxy.Send(EnvironmentID, "Engine.DataManager", "CommitTransaction", args);
				}				
			}
		}


		/// <summary>
		/// Loads and serializes specified data object into byte array.
		/// If error occurs, returns empty byte array and adds error to log
		/// </summary>
		/// <param name="dataObjectClassID">Class ID of the data object</param>
		/// <param name="dataObjectID">ID of the data object</param>
		/// <param name="dataObjectConfigID">Data object's config ID</param>
		/// <returns>Byte array</returns>
		public byte[] LoadDataObjectIntoArray(string dataObjectClassID, string dataObjectID, string dataObjectConfigID)
		{
			if (!m_initialized)
				Initialize();

			if (m_location != Location.APP_SERVER)
				throw new Exception("Data objects can be loaded into stream only in server process");
		
			try
			{
				// Load the object back in for shits and giggles:
				IDataObject dataObjectLoaded = (IDataObject) ClassFactory.GetFactory().Produce(dataObjectClassID);
				dataObjectLoaded.Initilize(dataObjectID, dataObjectConfigID, this);
				dataObjectLoaded.Load(this);

				MemoryStream stream = new MemoryStream(DataObject.AVERAGE_SIZE); // todo: figure out a way to create stream of exact size
				dataObjectLoaded.SaveToStream(stream);						
				return stream.GetBuffer();
			}
			catch(Exception e)
			{
				LogManager.Log(this, "Request to load data object failed: " + e.ToString());
			}

			return new byte[0];
		}


		/// <summary>
		/// Assists data reader in loading itself
		/// </summary>
		/// <param name="dataReader">Data reader</param>
		/// <exception cref="EInvalidParameter">If data reader pointer is null</exception>	
		public void LoadDataReader(DataReader dataReader)
		{
			if (!m_initialized)
				Initialize();

			if (dataReader == null)
				throw new EInvalidParameter(this, "dataReader");

			// If transaction is executing on application server side, we simply instruct the data reader to load itself from database connection
			if (m_location == Location.APP_SERVER)
				dataReader.LoadFromDB(m_dbConnection);
			else
			{
				// Use the remote transaction to load the reader:
				string readerSQL = dataReader.ComposeSQLCommand();
				string schema = dataReader.GetSchemaName();
				
				Object[] args = new Object[3];	// First element will be transaction.
				args[1] = schema;
				args[2] = readerSQL;
				byte[] array = (byte[]) m_serverConnectionProxy.Send(EnvironmentID, "Engine.DataManager", "LoadDataReader", args);
				MemoryStream stream = new MemoryStream(array);
				dataReader.LoadFromStream(stream);
			}
		}


		/// <summary>
		/// Creates data reader, executes the loading of it based on the SQL statement and then serializes the reader into MemoryStream.
		/// This function can only be executed on the server side.
		/// </summary>
		/// <param name="schema">Name of the schema being readed from</param>
		/// <param name="sql">SQL statement to be executed</param>
		/// <returns>Memory stram</returns>
		public MemoryStream StreamDataReader(string schema, string sql)
		{
			if (!m_initialized)
				Initialize();

			if (m_location != Location.APP_SERVER)
				throw new Exception("Cannot stream readers unless executing on applicaiton server");

			DataReader reader = new DataReader(schema);
			reader.LoadFromDB(m_dbConnection, sql);

			MemoryStream stream = new MemoryStream(DataReader.AVERAGE_SIZE); // todo: figure out a way to create stream of exact size
			reader.SaveToStream(stream);						

			return stream;
		}

 
	}
}
