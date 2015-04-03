using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Manages cache of DataObjects
	/// </summary>
	public class DataObjectCacheManager : Manager
	{
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected DataObjectCacheManager g_manager;

		/// <summary>
		/// Hash of data object caches keyed by environment IDs
		/// </summary>
		protected Hashtable m_enviromentCaches;


		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static DataObjectCacheManager()
		{
			g_manager = new DataObjectCacheManager();
		}


		/// <summary>
		/// Default constructor. Creates empty cache set.
		/// </summary>
		protected DataObjectCacheManager()
		{			
			m_enviromentCaches = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static DataObjectCacheManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes the manager.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns cached data object referenced by the given data object ID in given environment.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="dataObjectID">Data object ID</param>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>Data object reference or null if the data object was not found</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public IDataObject GetDataObject(string dataObjectID, string environmentID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (dataObjectID == null)
				throw new EInvalidParameter(this, "dataObjectID");

			// Find cache referenced environment
			Hashtable cache = (Hashtable) m_enviromentCaches[environmentID];
			if (cache == null)
				return null;				

			// Find right data object within the cache
			IDataObject dataObject = (IDataObject) cache[dataObjectID];
			return dataObject;
		}


		/// <summary>
		/// Returns cached data object referenced by the given data object ID in given environment.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="configID">Configuration ID</param>
		/// <param name="transaction">Transaction</param>
		/// <returns>Data object reference or null if the data object was not found</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters are null </exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public IDataObject GetDataObject(string dataObjectID, Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			return GetDataObject(dataObjectID, transaction.EnvironmentID);
		}


		/// <summary>
		/// Caches given data object within the environment 
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="dataObject">Data object to be cached</param>
		/// <param name="environmentID">Environment ID</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public void SetDataObject(IDataObject dataObject, string environmentID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (dataObject == null)
				throw new EInvalidParameter(this, "dataObject");

			// Find cache referenced environment
			Hashtable cache = (Hashtable) m_enviromentCaches[environmentID];
			if (cache == null)
			{
				cache = new Hashtable();
				m_enviromentCaches[environmentID] = cache;			
			}

			string dataObjectID = dataObject.GetID();
			cache[dataObjectID] = dataObject;
		}


		/// <summary>
		/// Caches given data object within the environment 
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="dataObject">Data object to be cached</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public void SetDataObject(IDataObject dataObject, Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			SetDataObject(dataObject, transaction.EnvironmentID);
		}


		/// <summary>
		/// Causes the manager to refresh (drop cache) (for all environments)
		/// </summary>
		public void Refresh()
		{
			m_enviromentCaches = new Hashtable();
		}
	}
}
