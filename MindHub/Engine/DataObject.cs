using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace Engine
{
	/// <summary>
	/// Simplest data-management entity. Allows for basic operations defined by the IDataObject interface.
	/// </summary>
	[Serializable]
	public class DataObject : IDataObject
	{
		/// <summary>
		/// Average data object size (used to optimize memory stream creation)
		/// </summary>
		public static int AVERAGE_SIZE = 4096;

		/// <summary>
		/// Class ID
		/// </summary>
		protected static string g_classID = "Engine.DataObject";	

		/// <summary>
		/// Singelton delegator instance reference
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Configuration table name
		/// </summary>
		static public string CONFIG_TABLE = "table";

		/// <summary>
		/// Storage table's primary key (object ID)
		/// </summary>
		static public string COLUMN_OBJID = "objid";
						
		/// <summary>
		/// Flag if of this instance will result in creation of a new DB record
		/// </summary>
		protected bool m_new;

		/// <summary>
		/// Flag for if the data object has been initialized with valid configuration
		/// </summary>
		protected bool m_initialized;

		/// <summary>
		/// Unique object ID
		/// </summary>
		protected string m_ID;	

		/// <summary>
		/// Configuration of this data object 
		/// </summary>
		protected Config m_config;

		/// <summary>
		/// Configuration ID of this data object 
		/// </summary>
		protected string m_configID;

		/// <summary>
		/// List of data elements
		/// </summary>
		protected Hashtable m_elements;

		/// <summary>
		/// List of table names
		/// </summary>
		protected ArrayList m_tables;

		/// <summary>
		/// On-load event sender
		/// </summary>		
		public event OnLoadHandler OnLoadEventSender;

		/// <summary>
		/// On-save event sender
		/// </summary>
		public event OnSaveHandler OnSaveEventSender;

		/// <summary>
		/// On-delete event sender
		/// </summary>
		public event OnDeleteHandler OnDeleteEventSender;

		/// <summary>
		/// On-create event sender
		/// </summary>
		public event OnCreateHandler OnCreateEventSender;

		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <return>Class ID</return>
		virtual public string GetClassID()
		{
			return g_classID;
		}


		/// <summary>
		/// Generates new data object ID (UUID)
		/// </summary>
		/// <returns>New UUID</returns>
		public static string GenerateID()
		{
			// TODO: this might not be the best way to create a DB GUID:
			return System.Guid.NewGuid().ToString();			
		}


		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new DataObject();
		}


		/// <summary>
		/// Get/Set accessors for flag indicating if the data object is new (does not exist in DB with given ID yet)
		/// </summary>
		public bool IsNew
		{
			get
			{ 
				return m_new; 
			}
			set 
			{
				m_new = value; 
			}
		}


		/// <summary>
		/// Indicates if the data object should be cached. Cache is stored in the DataObjectCacheManager. 
		/// If DataObject class is cached, when instance is loading, it will first attempt to load self from
		/// the cache manager.
		/// </summary>
		/// <returns>Flag for if this class is cached using the DataObjectCacheManager</returns>
		public virtual bool Cached()
		{
			return false;
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(DataObject.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);
		}


		/// <summary>
		/// Initializes instance with given configuration ID.
		/// Assumes new object is being created.
		/// </summary>
		/// <param name="configID">Configuration ID</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void Initilize(string configID, Transaction transaction)
		{
			if (configID == null)
				throw new EInvalidParameter(this, "configID");

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			m_ID = GenerateID();
			IsNew = true;

			// Invoke local event handler for derived classes
			OnCreate(transaction);

			// Application-wide event handling
			if (OnCreateEventSender != null)
				OnCreateEventSender(this, new EventArgs());

			// Find and initialize configuration object:
			ConfigManager configMgr = ConfigManager.GetManager();
			m_config = configMgr.GetConfig(configID, transaction);			
			m_configID = string.Copy(configID);
			m_elements = new Hashtable();
			InitilizeConfiguration(transaction);

			m_initialized = true;
		}


		/// <summary>
		/// Initializes instance with given object ID and configuration ID.
		/// Assumes existing object will be loaded or deleted.
		/// </summary>
		/// <param name="objectID">Object ID</param>
		/// <param name="configID">Configuration ID</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void Initilize(string objectID, string configID, Transaction transaction)
		{
			if (objectID == null)
				throw new EInvalidParameter(this, "objectID");

			if (configID == null)
				throw new EInvalidParameter(this, "configID");

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			m_ID = objectID;
			IsNew = false;

			// Retreive and initialize the configuration:
			ConfigManager configMgr = ConfigManager.GetManager();
			m_config = configMgr.GetConfig(configID, transaction);	
			m_configID = string.Copy(configID);
			m_elements = new Hashtable();		
			InitilizeConfiguration(transaction);
			
			m_initialized = true;
		}


		/// <summary>
		/// Initializes the internal configuration object
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If the data object's schema is not specified in the configuration</exception>
		virtual protected void InitilizeConfiguration(Transaction transaction)
		{
			m_tables = m_config.GetParamList(CONFIG_TABLE);
			if (m_tables.Count == 0)
				throw new EInvalidParameter(this, CONFIG_TABLE, "", "Configuration is missing object table name(s)");				
		}


		/// <summary>
		/// Loads the data object, assuming it was initialized with given object ID
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null or if the object was initialized with empty object ID</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void Load(Transaction transaction)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			if (m_ID.Length == 0)
				throw new EInvalidParameter(this, "m_ID", m_ID, "Data object was initialized with empty object ID");

			// See if this object is cached, if so, load it from cache:
			if (Cached())
			{
				DataObjectCacheManager manager = DataObjectCacheManager.GetManager();
				IDataObject dataObject = manager.GetDataObject(m_ID, transaction);
				if (dataObject != null)
				{
					LoadFromCache(dataObject);
					return;
				}
			}


			// Command the transaction to load this data object in:
			transaction.LoadDataObject(this);

			// Invoke local event handler for derived classes
			OnLoad(transaction);

			// Application-wide event handling
			if (OnLoadEventSender != null)
				OnLoadEventSender(this, new EventArgs());

			// See if this object is cached, if so, set it in cache:
			if (Cached())
			{
				DataObjectCacheManager manager = DataObjectCacheManager.GetManager();
				manager.SetDataObject(this, transaction);
			}
		}


		/// <summary>
		/// Saves the data object
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void Save(Transaction transaction)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			if (m_ID.Length == 0)
				throw new EInvalidParameter(this, "m_ID", m_ID, "Data object was initialized with empty object ID");

			// Invoke local event handler for derived classes
			OnSave(transaction);

			// Application-wide event handling
			if (OnSaveEventSender != null)
				OnSaveEventSender(this, new EventArgs());

			// Command the transaction to save this data object:
			transaction.SaveDataObject(this);

			// See if this object is cached, if so, set it in cache:
			if (Cached())
			{
				DataObjectCacheManager manager = DataObjectCacheManager.GetManager();
				manager.SetDataObject(this, transaction);
			}
		}


		/// <summary>
		/// Deletes the data object
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void Delete(Transaction transaction)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			if (m_ID.Length == 0)
				throw new EInvalidParameter(this, "m_ID", m_ID, "Data object was initialized with empty object ID");

			// Command the transaction to delete this data object:
			transaction.DeleteDataObject(this);

			// Invoke local event handler for derived classes
			OnDelete(transaction);

			// Application-wide event handling
			if (OnDeleteEventSender != null)
				OnDeleteEventSender(this, new EventArgs());
		}


		/// <summary>
		/// Sets its state based on a cached copy of this data object.
		/// Performs set of local data members by reference (unsafe).
		/// </summary>
		/// <param name="dataObject">Cached data object</param>
		virtual public void LoadFromCache(IDataObject cachedDataObject)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (cachedDataObject == null)
				throw new EInvalidParameter(this, "cachedDataObject");		

			DataObject dataObject = (DataObject) cachedDataObject;
			m_elements = dataObject.m_elements;
		}
		

		/// <summary>
		/// Saves self to stream
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void SaveToStream(MemoryStream stream)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (stream == null)
				throw new EInvalidParameter(this, "stream");

			BinaryFormatter serializer = new BinaryFormatter();
			serializer.Serialize(stream, m_elements);			
		}

	
		/// <summary>
		/// Loads self from stream
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void LoadFromStream(MemoryStream stream)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (stream == null)
				throw new EInvalidParameter(this, "stream");

			if (stream.Length == 0)
				throw new Exception("Cannot load data object from stream because stream is 0 in lenght");

			BinaryFormatter deserializer = new BinaryFormatter();
			m_elements = (Hashtable)(deserializer.Deserialize(stream));
		}


		/// <summary>
		/// Saves self to DB
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void SaveToDB(DBConnection connection)
		{
			// TODO: Implement mutli-table saves:

			if (!m_initialized)
				throw new ENotInitialized(this);

			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			IEnumerator itr = m_tables.GetEnumerator();
			while (itr.MoveNext())
			{
				string table = (string) itr.Current;

				// Using data writer, save self to database:
				DataWriter writer = new DataWriter(table);

				// Iterate through element + data value pairs:
				IDictionaryEnumerator itrElements = m_elements.GetEnumerator();
				bool performSave = false;
				while (itrElements.MoveNext())
				{
					string elementName = (string) itrElements.Key;
					DataElement element = (DataElement) itrElements.Value;

					// If this object was created to be new record, then we save all existing elements.
					// Else, we check the modified flag on each element before commanding the writer to save it.
					if (element.GetTable() == table && (IsNew || element.GetModified()))
					{
						writer.SetByReference(elementName, element.GetValue());
						performSave = true;
					}
				}
				
				// If any elements need saving, perform the save:
				if (performSave)
				{
					if (IsNew)
					{
						writer.SetByReference(COLUMN_OBJID, m_ID);
						writer.Insert(connection);
					}
					else
					{
						writer.Where(COLUMN_OBJID + " = '" + m_ID + "'");					
						writer.Update(connection);
					}
				}
			}
		}


		/// <summary>
		/// Loads self from DB
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void LoadFromDB(DBConnection connection)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			IEnumerator itr = m_tables.GetEnumerator();
			while (itr.MoveNext())
			{
				string table = (string) itr.Current;

				// Using data reader, get the record from the DB corresponding to the object's instance:
				DataReader reader = new DataReader(table);
				reader += DataReader.SQL_WILDCARD;
				reader.Where(COLUMN_OBJID + " = '" + m_ID + "'");
				reader.LoadFromDB(connection);
				if (!reader.Read())
					throw new EInvalidParameter(this, "m_ID", m_ID, "Provided object ID was not found in table " + table);

				if (m_elements.Count == 0)
					m_elements = reader.GetDataElements();  // Copy all elements by reference
				else
				{
					// Copy all elements by reference:
					Hashtable readElements = reader.GetDataElements();
					IDictionaryEnumerator itrElements = readElements.GetEnumerator();
					while (itrElements.MoveNext())
					{			
						string elementKey = (string) itrElements.Key;
						if (!m_elements.Contains(elementKey))
						{
							DataElement element = (DataElement) itrElements.Value;
							m_elements[elementKey] = element;
						}
					}
				}
			}
		}


		/// <summary>
		/// Deletes self from DB
		/// </summary>
		/// <param name="connection">connection</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public void DeleteFromDB(DBConnection connection)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			IEnumerator itr = m_tables.GetEnumerator();
			while (itr.MoveNext())
			{
				string table = (string) itr.Current;

				// Using data writer, set the delete flag and date:
				DataWriter writer = new DataWriter(table);
				writer.Where(COLUMN_OBJID + " = '" + m_ID + "'");
				writer.Delete(connection);
			}
		}


		/// <summary>
		/// Returns object's ID. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Object ID</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public string GetID()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			return string.Copy(m_ID);
		}


		/// <summary>
		/// Returns object's configuration ID. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Object's configuration ID</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public string GetConfigID()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			return string.Copy(m_configID);
		}


		/// <summary>
		/// Data element indexer. 
		/// Performs get/set by value (safe).
		/// </summary>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public DataValue this[string element]
		{
			get
			{
				if (!m_initialized)
					throw new ENotInitialized(this);

				if (element == null)
					throw new EInvalidParameter(this, "element");

				// Make sure to copy it by value
				return new DataValue(GetElement(element));
			}
			set
			{
				if (!m_initialized)
					throw new ENotInitialized(this);

				if (value == null)
					throw new EInvalidParameter(this, "value");

				// Make sure to copy everything by value (note: SetElement creates copy of 'element' argument)
				SetElement(element, new DataValue(value));
			}			
		}


		/// <summary>
		/// Sets value of a specified object element. 
		/// If the element does not exist among current elements, new element is created.
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="element">Element name</param>
		/// <param name="val">New data value for the element</param>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		/// <exception cref="EInvalidParameter">If element or val is null</exception>
		virtual protected void SetElement(string element, DataValue val)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (element == null)
				throw new EInvalidParameter(this, "element");

			if (val == null)
				throw new EInvalidParameter(this, "val");

			DataElement e = (DataElement) m_elements[element];
			if (e == null)
			{
				// TODO: Figure out how to handle multiple tables:
				e = new DataElement("T", element, val);
				m_elements[element] = e;
				e.SetModified(true);
			}
			else 
				e.SetValue(val);
		}


		/// <summary>
		/// Returns value of the element. 
		/// If referenced element does not exist, exception is thrown.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="element">Element name</param>
		/// <returns>Data value of the element</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		/// <exception cref="EInvalidParameter">If the element is not found on the current list of elements or if element is null</exception>
		virtual protected DataValue GetElement(string element)
		{			
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (element == null)
				throw new EInvalidParameter(this, "element");

			DataElement e = (DataElement) m_elements[element];
			if (e == null)
				throw new EInvalidParameter(this, "element", element, "Element does not exist in the data object");
			else 
				return e.GetValue();
		}


		/// <summary>
		/// Returns enumeration of DataElements. 
		/// The returned enumaration is of type IDictionaryEnumerator and can be casted.
		/// Enumerator elements are references to real data (does not perform copy by value before creating the enumerator)
		/// </summary>
		/// <returns>Enumeration of DataElements</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		virtual public IDictionaryEnumerator GetEnumerator()
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			return m_elements.GetEnumerator();
		}


		/// <summary>
		/// Returns hash of elements stored by the data object.
		/// Perform get by reference (unsafe). Any modifications to the returned object will affect the data object.
		/// </summary>
		/// <returns>Enumeration of DataElements</returns>
		public Hashtable GetElements()
		{
			return m_elements;
		}


		/// <summary>
		/// Local on-create event handler that can be overriden in derived classes
		/// </summary>
		/// <param name="transaction">transaction</param>
		virtual protected void OnCreate(Transaction transaction) {}


		/// <summary>
		/// Local on-load event handler that can be overriden in derived classes
		/// </summary>
		/// <param name="transaction">transaction</param>
		virtual protected void OnLoad(Transaction transaction) {}


		/// <summary>
		/// Local on-delete event handler that can be overriden in derived classes
		/// </summary>
		/// <param name="transaction">transaction</param>
		virtual protected void OnDelete(Transaction transaction) {}


		/// <summary>
		/// Local on-save event handler that can be overriden in derived classes
		/// </summary>
		/// <param name="transaction">transaction</param>
		virtual protected void OnSave(Transaction transaction) {}


		/// <summary>
		/// Gets operator settings for operator type specified by class ID.
		/// The settings are drawn from column named same as the class ID (with removed dot character).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <param name="classID">Operator class ID</param>
		/// <returns>String data set with operator settings</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		/// <exception cref="EInvalidParameter">If classID is invalid or null</exception>
		virtual public StringDataSet GetOperatorSettings(string classID)
		{			
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			string colum = string.Copy(classID).Replace('.', '_');
			string stringValue = GetElement(colum).GetStringByReference();
			StringDataSet dataSet = new StringDataSet(stringValue, StringDataSet.RepresentationType.AmpersandBased);			
			return dataSet;
		}


		/// <summary>
		/// Sets operator settings for operator type specified by class ID.
		/// The settings are drawn from column named same as the class ID (with removed dot character).
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="classID">Operator class ID</param>
		/// <param name="settings">String data set</param>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		/// <exception cref="EInvalidParameter">If classID is invalid or null or if settings is null</exception>
		virtual public void SetOperatorSettings(string classID, StringDataSet settings)
		{	
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			if (settings == null)
				throw new EInvalidParameter(this, "settings");

			string colum = string.Copy(classID).Replace('.', '_');
			SetElement(colum, settings.GetData(StringDataSet.RepresentationType.AmpersandBased));
		}


		/// <summary>
		/// Pushes out own state out to name-value collection in format: .table.column=value. Performs set by reference (unsafe).
		/// </summary>
		/// <param name="output">Target where data should be pushed out to</param>
		/// <param name="transaction">Transction</param>
		virtual public void PushData(NameValueCollection output, Transaction transaction)
		{
			IDictionaryEnumerator elements = m_elements.GetEnumerator();
			while (elements.MoveNext())
			{
				DataElement element = (DataElement) elements.Value;			
				string tableName = element.GetTable();
				string columnName = element.GetColumn();
				output["." + tableName + "." + columnName] = element.GetValue();
			}
		}

	
		/// <summary>
		/// Pulls in own state from name-value collection in format: .table.column=value. Performs set by value (safe).
		/// </summary>
		/// <param name="input">Input containing the state</param>
		/// <param name="transaction">Transaction</param>
		virtual public void PullData(NameValueCollection input, Transaction transaction)
		{
			IEnumerator inputs = input.GetEnumerator();
			while (inputs.MoveNext())
			{
				string variableName = (string) inputs.Current;
				if (variableName[0] == '.')
				{
					int nextDotPosition = variableName.IndexOf('.', 1);
					if (nextDotPosition > 0)
					{
						int stringLen = variableName.Length;
						string tableName = variableName.Substring(1, nextDotPosition - 1);
						string columnName = variableName.Substring(nextDotPosition + 1, stringLen - nextDotPosition - 1);
						if (tableName.Length > 0 && columnName.Length > 0)
						{
							string variableValue = input[variableName];
							SetElement(columnName, variableValue); // variableValue gets converted to DataValue upon this call so in effect this is by-value.
						}
					}
				}
			}
		}
	}
}
