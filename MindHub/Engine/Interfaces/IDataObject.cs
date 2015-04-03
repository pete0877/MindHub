using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace Engine
{
	// Delegates
	public delegate void OnLoadHandler(object sender, EventArgs args);
	public delegate void OnSaveHandler(object sender, EventArgs args);
	public delegate void OnDeleteHandler(object sender, EventArgs args);
	public delegate void OnCreateHandler(object sender, EventArgs args);

	/// <summary>
	/// Interface for basic operations on data.
	/// </summary>
	public interface IDataObject: ICreatable, IStreamStorable, IDBStorable
	{		
		/// <summary>
		/// Initializes instance with given configuration ID.
		/// Assumes new object is being created.
		/// </summary>
		/// <param name="configID">Configuration ID</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		void Initilize(string configID, Transaction transaction);


		/// <summary>
		/// Initializes instance with given object ID and configuration ID.
		/// Assumes existing object will be loaded or deleted
		/// </summary>
		/// <param name="objectID">Object ID</param>
		/// <param name="configID">Configuration ID</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		void Initilize(string objectID, string configID, Transaction transaction);


		/// <summary>
		/// Loads the data object, assuming it was initialized with given object ID
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null or if the object was initialized with empty object ID</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void Load(Transaction transaction);


		/// <summary>
		/// Saves the data object
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void Save(Transaction transaction);


		/// <summary>
		/// Deletes the data object
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void Delete(Transaction transaction);


		/// <summary>
		/// Returns object's ID. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Object ID</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		string GetID();


		/// <summary>
		/// Data element indexer. 
		/// Performs get/set by value (safe).
		/// </summary>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		DataValue this[string element]
		{
			get;
			set;
		}


		/// <summary>
		/// Get/Set accessors for flag indicating if the data object is new (does not exist in DB with given ID yet)
		/// </summary>
		bool IsNew
		{
			get;
			set;
		}


		/// <summary>
		/// Returns enumeration of DataElements. 
		/// The returned enumaration is of type IDictionaryEnumerator and can be casted.
		/// Enumerator elements are references to real data (does not perform copy by value before creating the enumerator)
		/// </summary>
		/// <returns>Enumeration of DataElements</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		IDictionaryEnumerator GetEnumerator();


		/// <summary>
		/// Returns hash of elements stored by the data object.
		/// Perform get by reference (unsafe). Any modifications to the returned object will affect the data object.
		/// </summary>
		/// <returns>Enumeration of DataElements</returns>
		Hashtable GetElements();


		/// <summary>
		/// Gets operator settings for operator type specified by class ID.
		/// The settings are drawn from column named same as the class ID (with removed dot character).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <param name="classID">Operator class ID</param>
		/// <returns>String data set with operator settings</returns>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		/// <exception cref="EInvalidParameter">If classID is invalid or null</exception>
		StringDataSet GetOperatorSettings(string classID);		


		/// <summary>
		/// Sets operator settings for operator type specified by class ID.
		/// The settings are drawn from column named same as the class ID (with removed dot character).
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="classID">Operator class ID</param>
		/// <param name="settings">String data set</param>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		/// <exception cref="EInvalidParameter">If classID is invalid or null or if settings is null</exception>
		void SetOperatorSettings(string classID, StringDataSet settings);


		/// <summary>
		/// Indicates if the data object should be cached. Cache is stored in the DataObjectCacheManager. 
		/// If DataObject class is cached, when instance is loading, it will first attempt to load self from
		/// the cache manager.
		/// </summary>
		/// <returns>Flag for if this class is cached using the DataObjectCacheManager</returns>
		bool Cached();


		/// <summary>
		/// Sets its state based on a cached copy of this data object.
		/// </summary>
		/// <param name="dataObject">Cached data object</param>
		void LoadFromCache(IDataObject cachedDataObject);


		/// <summary>
		/// Pulls in own state from name-value collection in format: .table.column=value. Performs set by value (safe).
		/// </summary>
		/// <param name="input">Input containing the state</param>
		/// <param name="transaction">Transaction</param>
		void PullData(NameValueCollection input, Transaction transaction);


		/// <summary>
		/// Pushes out own state out to name-value collection in format: .table.column=value. Performs set by reference (unsafe).
		/// </summary>
		/// <param name="output">Target where data should be pushed out to</param>
		/// <param name="transaction">Transction</param>
		void PushData(NameValueCollection output, Transaction transaction);
	}	
}